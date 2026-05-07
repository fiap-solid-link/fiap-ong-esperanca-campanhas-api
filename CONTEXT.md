# Contexto do Microsserviço Campanhas API

> Documento gerado a partir da documentação oficial do projeto **Conexão Solidária** (ONG Esperança Solidária / Hackathon 9NETT FIAP) localizada em `fiap-ong-esperanca-docs`. Reúne **todo o contexto necessário** para implementar os casos de uso deste microsserviço.
>
> **Fonte:** `C:\Users\barbara.pacheco\Documents\fiap\fiap-ong-esperanca-docs\docs` — `contexto.md`, `microsservicos/fiap-ong-esperanca-campanhas-api.md`, `modelagem/event-storming.md`, `arquitetura/index.md`, `decisoes-arquiteturais/registros/adr-{01..10}.md`, `execucao/tarefas.md`.

---

## 1. Visão geral da plataforma

A **Conexão Solidária** é uma plataforma digital para a ONG Esperança Solidária com 3 objetivos principais:

1. Cadastro/autenticação de doadores e gestores com **RBAC** (3 perfis: `Admin`, `GestorONG`, `Doador`).
2. **Gestão de campanhas** com ciclo de vida completo (Cadastrada → EmAndamento → Concluída/Cancelada).
3. **Processamento assíncrono de doações** via mensageria (RabbitMQ + Worker).
4. **Painel de transparência público** com dados em tempo real.

### Microsserviços (4)

| Serviço | Tipo | Bounded Context | Banco |
|---------|------|-----------------|-------|
| `fiap-ong-esperanca-identity-api` | ASP.NET Core Web API | Identidade e Acesso | PostgreSQL (`identity_db`) |
| **`fiap-ong-esperanca-campanhas-api`** *(este repo)* | ASP.NET Core Web API | **Campanhas + Transparência** | PostgreSQL (`campanhas_db`) + MongoDB (read models) |
| `fiap-ong-esperanca-doacao-worker` | .NET Worker Service | Doações | MongoDB (`doacoes_db`) |
| `fiap-ong-esperanca-gateway-api` | API Gateway (YARP) | Roteamento | — |

### Diagrama macro

```
Cliente → Gateway (YARP)
            ├─→ /api/identity/**     → identity-api
            ├─→ /api/campanhas/**    → campanhas-api  ◀── (este repo)
            ├─→ /api/transparencia/**→ campanhas-api
            └─→ /api/doacoes         → campanhas-api

campanhas-api ─Publish DoacaoRecebidaEvent────→ RabbitMQ ──→ worker
campanhas-api ◀────────Consume DoacaoProcessadaEvent────── RabbitMQ ◀── worker (Publish)
worker ─────→ MongoDB (doacoes + read models de transparência)
```

---

## 2. Bounded contexts cobertos pelo `campanhas-api`

Este serviço é responsável por **dois bounded contexts**:

### 2.1 Campanhas (write side)
- CRUD de campanhas (somente `GestorONG`).
- Máquina de estados completa.
- Endpoint de **intenção de doação** (síncrono) que valida e publica `DoacaoRecebidaEvent` no broker.
- **Consumer** de `DoacaoProcessadaEvent` que atualiza `ValorArrecadado` e dispara encerramento por meta quando aplicável.
- **Scheduler** (BackgroundService) para encerrar campanhas com `DataFim` expirada e alertar campanhas próximas do vencimento.

### 2.2 Transparência (read side)
- Endpoints públicos que **leem read models** projetados pelo Worker no MongoDB.
- Não gera eventos próprios — apenas consulta projeções.

---

## 3. Agregado `Campanha` — modelo de domínio

### 3.1 Campos
| Campo | Tipo | Notas |
|-------|------|-------|
| `Id` | Guid | Identidade |
| `Titulo` | string | obrigatório |
| `Descricao` | string | obrigatório |
| `DataInicio` | DateTime | |
| `DataFim` | DateTime | invariante: `DataFim > agora` na criação |
| `MetaFinanceira` | decimal | invariante: `> 0` |
| `ModoEncerramento` | enum | `PorData` \| `PorMeta` \| `PorDataOuMeta` |
| `Status` | enum | `Cadastrada` \| `EmAndamento` \| `Concluida` \| `Cancelada` |
| `ValorArrecadado` | decimal | atualizado via `DoacaoProcessadaEvent` |
| `IdGestor` | Guid | claim `sub` do JWT |

### 3.2 Máquina de estados (D2)

```
            criar
   ─────────────────────► Cadastrada
                              │ ativar (manual GestorONG)
                              ▼
                        EmAndamento ─── prorrogar (mesmo estado, DataFim nova > atual)
                          │     │
              concluir    │     │ cancelar
            (data ou meta)│     │
                          ▼     ▼
                       Concluida   Cancelada
                       (imutável)  (imutável)
```

- **Cadastrada**: estado inicial. Todos os campos editáveis. Pode ser **excluída/descartada** (sem evento de domínio).
- **EmAndamento**: aceita doações. **Apenas** prorrogação ou cancelamento.
- **Concluida**/**Cancelada**: imutáveis.
- Transições são **unidirecionais**.

### 3.3 Modos de encerramento (D3)

| Modo | Comportamento |
|------|---------------|
| `PorData` | Encerra automaticamente quando `DataFim` expira (scheduler). |
| `PorMeta` | Encerra automaticamente quando `ValorArrecadado >= MetaFinanceira` (reativo ao `DoacaoProcessadaEvent`). |
| `PorDataOuMeta` | Encerra pelo que acontecer primeiro. |

Período de **proximidade do vencimento**: 3 dias antes da `DataFim` (configurável via `appsettings.json`) — gera evento/log `CampanhaProximaDoVencimento`.

### 3.4 Comandos do agregado e regras

| Comando | Ator | Regra de negócio | Evento(s) |
|---------|------|------------------|-----------|
| `CriarCampanha` | `GestorONG` | `DataFim > agora`; `MetaFinanceira > 0`; `ModoEncerramento` válido | `CampanhaCriada` / `CriacaoCampanhaRejeitada` |
| `EditarCampanha` | `GestorONG` | Status deve ser `Cadastrada` | `CampanhaEditada` |
| `AtivarCampanha` | `GestorONG` | Status deve ser `Cadastrada` | `CampanhaAtivada` *(pivotal)* |
| `ProrrogarCampanha` | `GestorONG` | Status `EmAndamento` **e** nova `DataFim > DataFim atual` | `CampanhaProrrogada` |
| `CancelarCampanha` | `GestorONG` | Status `EmAndamento` | `CampanhaCancelada` |
| `VerificarVencimento` *(scheduler)* | Sistema | Campanhas `EmAndamento` com `DataFim` próxima e meta não atingida | `CampanhaProximaDoVencimento` |
| `EncerrarPorData` *(scheduler)* | Sistema | `EmAndamento` + `DataFim` expirada + modo `PorData` ou `PorDataOuMeta` | `CampanhaConcluidaPorData` |
| `AtualizarArrecadacao` *(policy)* | Sistema | Ao consumir `DoacaoProcessadaEvent` → `ValorArrecadado += Valor` | `ValorArrecadadoAtualizado` |
| `EncerrarPorMeta` *(policy)* | Sistema | `ValorArrecadado >= MetaFinanceira` + modo `PorMeta` ou `PorDataOuMeta` | `CampanhaConcluidaPorMeta` |

### 3.5 Invariantes (resumo)

- `DataFim > agora` na criação.
- `MetaFinanceira > 0`.
- Transições **unidirecionais** entre status.
- **Edição apenas em `Cadastrada`**.
- **Prorrogação apenas em `EmAndamento`** com nova `DataFim > DataFim atual`.
- **Cancelamento apenas em `EmAndamento`**.
- `Concluida` e `Cancelada` são imutáveis.

---

## 4. Fluxo de doações (D4 / D5 / D6)

> O agregado `Doacao` é **gerenciado pelo Worker** (MongoDB), mas a **intenção** entra pelo `campanhas-api`.

### 4.1 Caminho da doação (síncrono → assíncrono)

```
1. Doador → POST /api/doacoes (IdCampanha, Valor)
2. campanhas-api valida (síncrono):
     - Campanha está EmAndamento?
     - Meta não atingida (conforme ModoEncerramento)?
     - Valor > 0?
3. Se válida:
     - Emite IntencaoDoacaoRecebida (domain event interno)
     - Publica DoacaoRecebidaEvent no exchange esperanca.doacoes (rk: recebida)
       Payload: IdDoacao, IdCampanha, IdDoador, Valor, DataIntencao, IdempotencyKey
4. Se inválida:
     - DoacaoRecusada (motivo)
     - HTTP 400 com justificativa

5. [Worker] Consome doacoes-recebidas
     - Persiste no MongoDB (idempotência via IdempotencyKey)
     - Publica DoacaoProcessadaEvent (rk: processada)

6. [campanhas-api] Consome doacoes-processadas
     - Carrega Campanha
     - ValorArrecadado += Valor → ValorArrecadadoAtualizado
     - Se ModoEncerramento (PorMeta | PorDataOuMeta) e ValorArrecadado >= MetaFinanceira
        → CampanhaConcluidaPorMeta (status → Concluida)
```

### 4.2 Decisões importantes (hot spots)

- **Doações acima da meta SÃO ACEITAS** (D5). Se a intenção passou pela validação síncrona, ela é processada. O encerramento por meta é verificado **reativamente** ao consumir `DoacaoProcessadaEvent`.
- **Idempotência** é responsabilidade do Worker (via `IdempotencyKey`), mas o `campanhas-api` deve ser **reentrante** ao processar `DoacaoProcessadaEvent` — se a mesma mensagem chegar duas vezes, não somar duas vezes.
- **Concorrência** no `ValorArrecadado` é mitigada com prefetch=1 no consumer e/ou controle de concorrência otimista no EF Core.
- **Política de retry do Worker**: 3 tentativas com backoff exponencial (1s, 4s, 16s). Após esgotar → DLQ + evento `ProcessamentoDoacaoFalhou`.

---

## 5. Endpoints HTTP

| Método | Rota | Acesso | Descrição |
|--------|------|--------|-----------|
| `POST` | `/api/campanhas` | `GestorONG` | Criar campanha |
| `PUT` | `/api/campanhas/{id}` | `GestorONG` | Editar campanha (status `Cadastrada`) |
| `POST` | `/api/campanhas/{id}/ativar` | `GestorONG` | Ativar campanha |
| `POST` | `/api/campanhas/{id}/prorrogar` | `GestorONG` | Prorrogar `DataFim` |
| `POST` | `/api/campanhas/{id}/cancelar` | `GestorONG` | Cancelar campanha |
| `GET` | `/api/campanhas/{id}` | `GestorONG` | Detalhe (gestão) |
| `GET` | `/api/campanhas` | `GestorONG` | Listar campanhas do gestor |
| `POST` | `/api/doacoes` | `Doador` | Enviar intenção de doação |
| `GET` | `/api/transparencia/painel` | Público | Visão macro: total geral + Top 3 doadores |
| `GET` | `/api/transparencia/campanhas` | Público | Lista (ativas primeiro, depois encerradas) |
| `GET` | `/api/transparencia/campanhas/{id}` | Público | Detalhe + doações anonimizadas |
| `GET` | `/health` | Público | Health check (PostgreSQL + MongoDB + RabbitMQ) |

> **GestorONG acumula perfil Doador** (D1) — pode enviar doações.

---

## 6. Persistência (ADR-04)

### 6.1 PostgreSQL — `campanhas_db`
- Tabelas: `campanhas`, `historico` (auditoria).
- Acesso via **EF Core + Npgsql**.
- `IAppDbContext` (Application) ↔ `CampanhaDbContext` (Infrastructure).
- Configurações por entidade descobertas via `ApplyConfigurationsFromAssembly` (já adotado).

### 6.2 MongoDB — `doacoes_db` (read side)
- Collections **lidas** pelo `campanhas-api` (transparência):
  - `painel_macro` — total geral arrecadado + Top 3 doadores.
  - `lista_campanhas` — campanhas com meta, valor arrecadado, status, data encerramento.
  - `campanha_detalhe` — detalhe + doações anonimizadas (valor + data).
- **Quem escreve nesses read models é o Worker**, não o `campanhas-api`. Aqui só temos repositórios de leitura.
- Driver: `MongoDB.Driver`.
- Interface a definir na Application: `ITransparenciaReadRepository`. Implementação na Infrastructure: `TransparenciaMongoRepository`.

### 6.3 Read Model relacional `CampanhaGestaoView`
- Campos: todos os atributos da campanha + histórico de alterações.
- Consumidor: `GestorONG` autenticado.
- Armazenamento: PostgreSQL (mesmo banco do agregado).

---

## 7. Mensageria (ADR-05)

### 7.1 Topologia RabbitMQ

| Exchange | Tipo | Fila | Producer | Consumer |
|----------|------|------|----------|----------|
| `esperanca.doacoes` | direct | `doacoes-recebidas` | **campanhas-api** | worker |
| `esperanca.doacoes` | direct | `doacoes-processadas` | worker | **campanhas-api** |
| `esperanca.doacoes.dlx` | fanout | `doacoes-recebidas-dlq` | RabbitMQ (auto) | — |

### 7.2 Configuração de resiliência

| Parâmetro | Valor |
|-----------|-------|
| Retry count | 3 |
| Backoff | Exponencial — 1s, 4s, 16s |
| DLQ | `doacoes-recebidas-dlq` (via Dead Letter Exchange) |
| TTL da mensagem | Sem TTL (doações validadas devem sempre ser processadas) |
| Prefetch (Worker e consumer do `campanhas-api`) | 1 (sequencial → simplifica consistência) |
| ACK | Manual após persistência + publicação |

### 7.3 Contratos (eventos de integração — **shared records**)

```csharp
// Publicado por campanhas-api, consumido pelo worker
public record DoacaoRecebidaEvent(
    Guid IdDoacao,
    Guid IdCampanha,
    Guid IdDoador,
    decimal Valor,
    DateTime DataIntencao,
    Guid IdempotencyKey
);

// Publicado pelo worker, consumido por campanhas-api
public record DoacaoProcessadaEvent(
    Guid IdDoacao,
    Guid IdCampanha,
    decimal Valor,
    DateTime DataProcessamento
);
```

> A estratégia de compartilhamento (cópia direta nos repos ou pacote NuGet local) está pendente — para o MVP, manter records C# idênticos nos dois repositórios e cobrir com **testes de contrato** (serialização/desserialização bidirecional, ADR-10 nível 3).

---

## 8. Autenticação e autorização (ADR-06)

| Aspecto | Decisão |
|---------|---------|
| Emissor | `identity-api` (HMAC-SHA256, chave simétrica) |
| Validação | `campanhas-api` valida localmente com a **mesma signing key** (env var `Jwt__SecretKey`) — **sem chamadas runtime** ao `identity-api` |
| Claims | `sub` (userId), `email`, `roles` (array — pode conter `Admin`, `GestorONG`, `Doador`) |
| Expiração | Access token 30 min; refresh 7 dias |
| Autorização | `[Authorize(Roles = "GestorONG")]` etc. nos controllers |

### Permissões relevantes para este serviço

| Recurso | Admin | GestorONG | Doador | Visitante |
|---------|:-----:|:---------:|:------:|:---------:|
| Criar/Editar/Ativar/Prorrogar/Cancelar Campanha | — | ✅ | — | — |
| Enviar doação | — | ✅* | ✅ | — |
| Consultar transparência | ✅ | ✅ | ✅ | ✅ (público) |

\* `GestorONG` acumula perfil `Doador` (D1).

> **GestorONG só pode operar suas próprias campanhas** — implementar checagem `IdGestor == claim.sub` nos handlers de edição/ativação/prorrogação/cancelamento.

---

## 9. Arquitetura interna (ADR-03)

**Clean Architecture (4 camadas) + Vertical Slice na Application** (já adotado neste repo).

```
src/
  Esperanca.Campanha.Domain          (entidades, VOs, enums, regras puras)
  Esperanca.Campanha.Application     (use cases, MediatR handlers, DTOs, validators, abstrações)
  Esperanca.Campanha.Infrastructure  (EF Core, Npgsql, MongoDB.Driver, RabbitMQ.Client, JWT)
  Esperanca.Campanha.WebApi          (Controllers, middleware, composition root, Swagger)
test/
  Esperanca.Campanha.UnitTests       (xUnit + NSubstitute + EF Core InMemory)
```

### Convenções já consolidadas no repo
- **Composition root via módulos**: cada camada expõe `XxxModule.ConfigureServices(...)`. `Program.cs` chama somente `CampanhaWebApiModule`.
- **CQRS via MediatR**: handlers em `Application/{Feature}/...`. Controllers finos.
- **Pipeline `ValidationBehavior<TRequest,TResponse>`**: roda `IValidator<TRequest>` antes do handler. Lança `FluentValidation.ValidationException`.
- **`ValidationExceptionMiddleware`**: traduz `ValidationException` para 400 com payload `{ titulo: "Campanha:900", erros: { ... } }`.
- **Result pattern** (`Application/_Shared/Results/Result<T>`): `Ok`, `Created`, `Fail`, `NotFound`, `Unauthorized` + `StatusCode`. Controllers traduzem `Result.StatusCode` para `IActionResult`.
- **Acesso a dados**: handlers dependem de `IAppDbContext` (Application), não de `CampanhaDbContext` (Infrastructure).
- **Localização**: mensagens em `Application/_Shared/Localization/{pt-BR,en}.json` indexadas por código (`Campanha:NNN`); constantes em `CampanhaErrorCodes`; resolução via `IAppLocalizer[code]`.
- **Versionamento NuGet centralizado** em `Directory.Packages.props`.

> ⚠️ **Estado atual**: o último commit (`aa59632`) renomeou os projetos para o layout acima e o Domain ficou apenas com `Class1.cs` (placeholder). A entidade `Campanha`, handlers, validators e controllers que existiam na estrutura antiga **ainda precisam ser portados** para os 4 projetos atuais.

---

## 10. Casos de uso a implementar (vertical slices na Application)

> Cada slice = pasta autocontida com `Command`/`Query`, `Handler`, `Validator`, e DTOs próprios.

### Campanhas (write)
1. `CriarCampanha` — POST `/api/campanhas`.
2. `EditarCampanha` — PUT `/api/campanhas/{id}`.
3. `AtivarCampanha` — POST `/api/campanhas/{id}/ativar`.
4. `ProrrogarCampanha` — POST `/api/campanhas/{id}/prorrogar`.
5. `CancelarCampanha` — POST `/api/campanhas/{id}/cancelar`.

### Campanhas (read — gestão)
6. `ObterCampanha` — GET `/api/campanhas/{id}`.
7. `ListarCampanhasGestor` — GET `/api/campanhas`.

### Doações (intenção, síncrono)
8. `EnviarIntencaoDoacao` — POST `/api/doacoes`.
   - Valida campanha (`EmAndamento`, valor > 0, regras do `ModoEncerramento`).
   - Publica `DoacaoRecebidaEvent` via `IDoacaoPublisher` (Application) → `RabbitMqDoacaoPublisher` (Infra).

### Doações (processada, assíncrono — consumer)
9. `ProcessarDoacaoProcessada` — handler interno disparado pelo `DoacaoProcessadaConsumerService` (Infra `BackgroundService`).
   - `AtualizarArrecadacao` (policy) → `ValorArrecadadoAtualizado`.
   - Se atingiu meta + modo `PorMeta`/`PorDataOuMeta` → `EncerrarPorMeta` → `CampanhaConcluidaPorMeta`.

### Scheduler (BackgroundService, timer ~1 min)
10. `VerificarVencimento` — campanhas `EmAndamento` com `DataFim` próxima (3 dias) e meta não atingida → log/alerta `CampanhaProximaDoVencimento`.
11. `EncerrarPorData` — campanhas `EmAndamento` com `DataFim` expirada e modo `PorData` ou `PorDataOuMeta` → `CampanhaConcluidaPorData`.

### Transparência (read public, MongoDB)
12. `ConsultarPainelMacro` — GET `/api/transparencia/painel`.
13. `ConsultarListaCampanhas` — GET `/api/transparencia/campanhas`.
14. `ConsultarDetalheCampanha` — GET `/api/transparencia/campanhas/{id}`.

---

## 11. Dependências externas e pacotes

```
.NET 10 · ASP.NET Core 10 · EF Core 10 + Npgsql · MongoDB.Driver
MediatR · FluentValidation · RabbitMQ.Client
Microsoft.AspNetCore.Authentication.JwtBearer
Serilog (+ Sinks.Console, Sinks.ApplicationInsights) · Swashbuckle
xUnit · NSubstitute · (Testcontainers para integração — futuro)
```

Versões centralizadas em `Directory.Packages.props`. Configurações comuns (`TargetFramework=net10.0`, `Nullable=enable`, `ImplicitUsings=enable`) em `Directory.Build.props`.

---

## 12. Configuração e segredos

| Chave | Origem | Observação |
|-------|--------|------------|
| `ConnectionStrings:CampanhaDb` | `appsettings.json` / `ConnectionStrings__CampanhaDb` (env) | PostgreSQL `campanhas_db` |
| `ConnectionStrings:DoacoesMongo` | env | MongoDB `doacoes_db` (read models) |
| `RabbitMq:Host`, `:User`, `:Password` | env | Broker |
| `Jwt:SecretKey`, `:Issuer`, `:Audience`, `:AccessTokenExpirationMinutes` | env (K8s Secret em prod) | **Mesma chave** que o `identity-api` |
| `Scheduler:ProximidadeVencimentoEmDias` | `appsettings.json` | Default 3 |
| `Scheduler:IntervaloEmSegundos` | `appsettings.json` | Default 60 |

---

## 13. Observabilidade (ADR-08)

- **Logging**: Serilog estruturado (Console em dev, Application Insights em prod). Propagar `correlation-id` nas mensagens RabbitMQ.
- **Métricas relevantes para este serviço**:
  - Request duration p50/p95/p99 dos endpoints.
  - Active campaigns (campanhas `EmAndamento`).
  - Queue depth de `doacoes-recebidas`/`doacoes-processadas`.
  - DLQ count (alerta se > 0).
  - Error rate 5xx.
- **Health check** (`/health`): PostgreSQL `campanhas_db` + MongoDB `doacoes_db` + RabbitMQ.

---

## 14. Estratégia de testes (ADR-10)

| Nível | Alvo | Stack |
|-------|------|-------|
| **Unit** | Entidade `Campanha` (transições + invariantes), Value Objects, Validators, Handlers | xUnit + NSubstitute + EF Core InMemory |
| **Integração** | Repositório EF Core contra PostgreSQL real, endpoints via `WebApplicationFactory`, consumer RabbitMQ | xUnit + Testcontainers (PostgreSQL, MongoDB, RabbitMQ) |
| **Contrato** | Serialização/desserialização bidirecional de `DoacaoRecebidaEvent` e `DoacaoProcessadaEvent` | xUnit |

> **Regra obrigatória do projeto**: nenhuma issue é "concluída" sem `dotnet test` passando. Cobertura mínima esperada — 12+ cenários de transição da entidade `Campanha`, handlers de `CriarCampanha` e `AtivarCampanha`, validators dos comandos.

---

## 15. Mapa rápido — eventos de domínio do `campanhas-api`

| Evento | Disparado por | Pivotal? |
|--------|---------------|----------|
| `CampanhaCriada` | `CriarCampanha` | — |
| `CampanhaEditada` | `EditarCampanha` | — |
| `CampanhaAtivada` | `AtivarCampanha` | 🔴 Sim |
| `CampanhaProrrogada` | `ProrrogarCampanha` | — |
| `CampanhaProximaDoVencimento` | Scheduler | — |
| `CampanhaConcluidaPorData` | Scheduler | — |
| `CampanhaConcluidaPorMeta` | Policy reativa a `DoacaoProcessadaEvent` | — |
| `CampanhaCancelada` | `CancelarCampanha` | — |
| `IntencaoDoacaoRecebida` | `EnviarIntencaoDoacao` (validação OK) | — |
| `DoacaoRecusada` | `EnviarIntencaoDoacao` (validação falhou) | — |
| `DoacaoRecebidaEvent` (publicado) | `EnviarIntencaoDoacao` → broker | 🔴 Sim |
| `ValorArrecadadoAtualizado` | Consumer de `DoacaoProcessadaEvent` | — |

---

## 16. Issue tracker do hackathon (frentes que tocam este repo)

- **Issue #3** — Domínio + CRUD (entidade `Campanha`, máquina de estados, 5 use cases CRUD, listagem/detalhe, controller com JWT).
- **Issue #4** — Doações + Mensageria (`EnviarIntencaoDoacao`, `IDoacaoPublisher`, `RabbitMqDoacaoPublisher`, `DoacaoProcessadaConsumerService`, encerramento por meta).
- **Issue #5** — Transparência + Scheduler (`ITransparenciaReadRepository`, `TransparenciaMongoRepository`, `CampanhaSchedulerService`, 3 endpoints públicos).

Acceptance criteria-chave (de #3 a #5):
- GestorONG cria/edita/ativa/prorroga/cancela com validações.
- JWT do `identity-api` validado corretamente.
- Doação rejeitada se campanha não está `EmAndamento` ou `Valor <= 0`.
- Consumer atualiza `ValorArrecadado` e encerra por meta nos modos `PorMeta`/`PorDataOuMeta`.
- Scheduler encerra `EmAndamento` com `DataFim` expirada e loga alerta para próximas do vencimento.
- Endpoints de transparência respondem **sem autenticação**.
- `dotnet test` passa.

---

## 17. Referências

- `fiap-ong-esperanca-docs/docs/contexto.md`
- `fiap-ong-esperanca-docs/docs/microsservicos/fiap-ong-esperanca-campanhas-api.md`
- `fiap-ong-esperanca-docs/docs/microsservicos/index.md`
- `fiap-ong-esperanca-docs/docs/modelagem/event-storming.md`
- `fiap-ong-esperanca-docs/docs/modelagem/index.md`
- `fiap-ong-esperanca-docs/docs/arquitetura/index.md`
- `fiap-ong-esperanca-docs/docs/decisoes-arquiteturais/registros/adr-{01..10}.md`
- `fiap-ong-esperanca-docs/docs/execucao/tarefas.md`
- Repo local: `CLAUDE.md` (convenções já adotadas, comandos comuns, estado atual da estrutura).
