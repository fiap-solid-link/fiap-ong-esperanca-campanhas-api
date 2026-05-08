# Plano de Implementação — `campanhas-api`

> Roadmap em 10 fases sequenciais. Cada fase encerra com `dotnet test` verde. Convenções já consolidadas no repo (ver `CLAUDE.md`/`CONTEXT.md`): MediatR, FluentValidation, `Result<T>`, `ValidationBehavior`, `ValidationExceptionMiddleware`, `IAppDbContext`, `IAppLocalizer`, `CampanhaErrorCodes`, módulos de composition root, `Directory.Packages.props` centralizado.

## Decisões confirmadas (07/05/2026)

1. **Erros de domínio** → `DomainException(codigo, mensagem)` na entidade; handlers convertem para `Result.Fail`.
2. **Idempotência da arrecadação** → tabela `arrecadacoes_processadas (id_doacao PK)` no Postgres.
3. **OutBox pattern** → não no MVP; publicar direto no handler.
4. **Transparência** → entra no escopo agora; seed mock no Mongo via `init-mongo.js`.
5. **`ICurrentUser`** → modelar na Fase 2 (essencial para isolamento por gestor).

---

## ✅ Fase 1 — Domain (concluída)

**Status:** ✔️ Concluída · 44 testes passando

**Entregues:**
- `src/Esperanca.Campanha.Domain/_Shared/DomainException.cs`
- `src/Esperanca.Campanha.Domain/Campanhas/StatusCampanha.cs` (`Cadastrada` | `EmAndamento` | `Concluida` | `Cancelada`)
- `src/Esperanca.Campanha.Domain/Campanhas/ModoEncerramento.cs` (`PorData` | `PorMeta` | `PorDataOuMeta`)
- `src/Esperanca.Campanha.Domain/Campanhas/CampanhaErros.cs` (códigos `Campanha:101`–`302`)
- `src/Esperanca.Campanha.Domain/Campanhas/Campanha.cs` (factory `Criar`, métodos `Editar`/`Ativar`/`Prorrogar`/`Cancelar`/`RegistrarArrecadacao`/`ConcluirPorData`/`ConcluirPorMeta`, predicados `AtingiuMeta`/`PodeConcluirPorData`/`PodeConcluirPorMeta`/`EstaProximaDoVencimento`)
- `test/Esperanca.Campanha.UnitTests/Domain/Campanhas/CampanhaTests.cs` (44 cenários, alias `using CampanhaAgg = ...` para resolver CS0118)
- Removido `Class1.cs` placeholder.

---

## ✅ Fase 2 — Abstrações Application (concluída)

**Status:** ✔️ Concluída · 57 testes passando

**Entregues:**
- `src/Esperanca.Campanha.Application/_Shared/IDateTimeProvider.cs`
- `src/Esperanca.Campanha.Application/_Shared/ICurrentUser.cs` (`UserId`, `Roles`, `EstaNaRole(string)`)
- `src/Esperanca.Campanha.Application/_Shared/Localization/CampanhaErrorCodes.cs` (+`ErroValidacao:900`, `CampanhaNaoEncontrada:901`, `AcessoNaoAutorizado:902`)
- `src/Esperanca.Campanha.Application/_Shared/Results/Result.cs` (+`Forbidden`)
- `src/Esperanca.Campanha.Infrastructure/_Shared/SystemDateTimeProvider.cs`
- `src/Esperanca.Campanha.Infrastructure/_Shared/CurrentUserAccessor.cs` (extrai `ClaimTypes.NameIdentifier` / `"sub"` e `ClaimTypes.Role`)
- `src/Esperanca.Campanha.Infrastructure/Campanhas/CampanhaConfiguration.cs` (mapping + índices `IdGestor`/`Status`)
- `CampanhaInfrastructureModule`: registra `IDateTimeProvider`, `ICurrentUser`, `IHttpContextAccessor`
- `test/Esperanca.Campanha.UnitTests/Infrastructure/DateTimeProvider/SystemDateTimeProviderTest.cs` (2 cenários)
- `test/Esperanca.Campanha.UnitTests/Infrastructure/CurrentUser/CurrentUserAccessorTest.cs` (10 cenários)
- `Directory.Packages.props` + `UnitTests.csproj`: adicionado `MockQueryable.NSubstitute 7.0.0` + referência a Infrastructure

---

## ✅ Fase 3 — Slices Write (CRUD da Campanha) (concluída)

**Status:** ✔️ Concluída · 89 testes passando

**Entregues:**
- `src/Esperanca.Campanha.Application/Campanhas/_Shared/CampanhaDto.cs`
- `src/Esperanca.Campanha.Application/Campanhas/Criar/{CriarCampanhaCommand,Handler,Validator}.cs`
- `src/Esperanca.Campanha.Application/Campanhas/Editar/{EditarCampanhaCommand,Handler,Validator}.cs`
- `src/Esperanca.Campanha.Application/Campanhas/Ativar/{AtivarCampanhaCommand,Handler}.cs`
- `src/Esperanca.Campanha.Application/Campanhas/Prorrogar/{ProrrogarCampanhaCommand,Handler,Validator}.cs`
- `src/Esperanca.Campanha.Application/Campanhas/Cancelar/{CancelarCampanhaCommand,Handler}.cs`
- `src/Esperanca.Campanha.Infrastructure/_Shared/ResourceAppLocalizer.cs` (IAppLocalizer + JSON embutido)
- `src/Esperanca.Campanha.WebApi/Campanhas/CampanhaController.cs` (`[Authorize(Roles = "GestorONG")]`)
- `src/Esperanca.Campanha.WebApi/_Shared/Extensions/ResultExtensions.cs`
- `test/...Application/Campanhas/{Criar,Editar,Ativar,Prorrogar,Cancelar}/` (handlers + validators)
- Localizações `pt-BR.json`/`en.json` expandidas com códigos `Campanha:101`–`302`

---

## ✅ Fase 4 — Slices Read (gestão) (concluída)

**Status:** ✔️ Concluída · 103 testes passando

**Entregues:**
- `src/Esperanca.Campanha.Application/Campanhas/Obter/{ObterCampanhaQuery,Handler}.cs` — GET `/api/campanhas/{id}` filtrado pelo `ICurrentUser.UserId` (campanhas de outros gestores retornam 404).
- `src/Esperanca.Campanha.Application/Campanhas/Listar/{ListarCampanhasGestorQuery,Handler,Validator}.cs` + `PaginaCampanhasDto.cs` — GET `/api/campanhas` com paginação (`Pagina`, `TamanhoPagina ≤ 100`) e filtros opcionais (`Status`, intervalo `DataInicioDe..DataInicioAte`); ordenação por `DataInicio desc, Id asc`.
- `CampanhaController`: endpoints `GET /api/campanhas` e `GET /api/campanhas/{id}` (já protegidos pelo `[Authorize(Roles = "GestorONG")]` da classe).
- `test/...Application/Campanhas/Obter/` (3 cenários do handler) e `test/...Application/Campanhas/Listar/` (5 cenários do handler + 6 do validator).

---

## ✅ Fase 5 — Infrastructure write side + Controller + JWT (concluída)

**Status:** ✔️ Concluída · 106 testes passando (103 unit + 3 smoke)

**Entregues:**
- `src/Esperanca.Campanha.Infrastructure/Migrations/20260508205247_InicialCampanha.cs` — migration inicial criando tabela `campanhas` (chave `Id`, índices `IdGestor` e `Status`, `numeric(18,2)` para meta/arrecadação).
- `src/Esperanca.Campanha.WebApi/_Shared/Authentication/{JwtOptions,JwtAuthenticationExtensions}.cs` — JWT Bearer HMAC-SHA256 com `Jwt:*` validando issuer/audience/lifetime/signing key. Configuração feita via `services.AddOptions<JwtBearerOptions>().Configure<IConfiguration>(...)` para que o `WebApplicationFactory` consiga sobrescrever `Jwt:*` em testes.
- `CampanhaWebApiModule` agora chama `services.AddCampanhaJwtAuthentication(configuration)` (composition root continua único — `Program.cs` apenas dispara `CampanhaWebApiModule`).
- `CampanhaController` permanece fino: cada endpoint apenas dispara o `ISender.Send` e traduz o `Result` via `ToActionResult`.
- `test/...UnitTests/WebApi/Smoke/`:
  - `CampanhaWebApplicationFactory` substitui `CampanhaDbContext` por InMemory (limpa todos os descritors `Microsoft.EntityFrameworkCore`/`Npgsql` antes de re-registrar) e injeta `Jwt:*` via `AddInMemoryCollection`; remove o health check do Postgres.
  - `JwtTokenFactory` emite tokens HMAC-SHA256 com `sub` + role.
  - `CampanhaSmokeTest` cobre os três cenários: `POST /api/campanhas` retorna 201 com role `GestorONG`, 401 sem token, 403 com role errada.
- `Directory.Packages.props` + `UnitTests.csproj`: adicionados `Microsoft.AspNetCore.Mvc.Testing 10.0.5` e `Microsoft.EntityFrameworkCore.InMemory 10.0.5`.

---

## ✅ Fase 6 — Doações (intenção, síncrona) (concluída)

**Status:** ✔️ Concluída · 117 testes passando

**Entregues:**
- `src/Esperanca.Campanha.Application/Doacoes/_Shared/Contracts/DoacaoRecebidaEvent.cs` — record com `IdDoacao`, `IdCampanha`, `IdDoador`, `Valor`, `DataIntencao`, `IdempotencyKey`.
- `src/Esperanca.Campanha.Application/Doacoes/_Shared/IDoacaoPublisher.cs` — abstração `PublicarRecebidaAsync(evento, ct)`.
- `src/Esperanca.Campanha.Application/Doacoes/EnviarIntencao/{EnviarIntencaoDoacaoCommand,Handler,Validator,IntencaoDoacaoDto}.cs`:
  - Valida `Valor > 0` (validator) e `IdCampanha` não vazio.
  - Handler busca campanha (404 se inexistente), confere `Status == EmAndamento` (400 reusando `Campanha:301` `ArrecadacaoSomenteEmAndamento`), gera `IdDoacao`/`IdempotencyKey`, publica e retorna **202 Accepted** com `IntencaoDoacaoDto(IdDoacao, IdempotencyKey, DataIntencao)`.
- `src/Esperanca.Campanha.Application/_Shared/Results/Result.cs` — adicionado `Accepted(T)` (202).
- `src/Esperanca.Campanha.WebApi/Doacoes/DoacaoController.cs` — `POST /api/doacoes` com `[Authorize(Roles = "Doador,GestorONG")]`.
- **Infra RabbitMQ** (`Infrastructure/Doacoes/RabbitMq/`):
  - `RabbitMqOptions` (Host/Port/User/Password/VirtualHost/Exchange/Queue/RoutingKey/DeadLetterExchange/DeadLetterQueue).
  - `RabbitMqDoacaoPublisher` (singleton, lazy-init de connection/channel) declara topologia idempotentemente na primeira publicação: exchange `esperanca.doacoes` (direct, durable), fila `doacoes-recebidas` (durable, com `x-dead-letter-exchange = esperanca.doacoes.dlx`), DLX `esperanca.doacoes.dlx` (fanout) e DLQ `doacoes-recebidas-dlq`. Mensagens persistentes, `MessageId = IdDoacao`, `CorrelationId = IdempotencyKey`.
  - Registrado em `CampanhaInfrastructureModule` via `services.AddSingleton<IDoacaoPublisher, RabbitMqDoacaoPublisher>()`.
  - Pacote `RabbitMQ.Client 7.0.0` adicionado ao `Directory.Packages.props` e ao csproj de Infrastructure.
  - `appsettings.json` ganhou seção `RabbitMq:*` com defaults para localhost/guest.
- **Testes** (`test/.../Application/Doacoes/`):
  - `EnviarIntencao/EnviarIntencaoDoacaoHandlerTest.cs` — 4 cenários (publica + retorna 202; campanha não encontrada; campanha cadastrada → 400; publisher exception propaga).
  - `EnviarIntencao/EnviarIntencaoDoacaoValidatorTest.cs` — 4 cenários (válido, valor zero, valor negativo, IdCampanha vazio).
  - `_Shared/Contracts/DoacaoRecebidaEventContractTest.cs` — round-trip JSON, presença de todos os campos, leitura de payload externo (interoperabilidade com worker).
  - Mock `DoacaoPublisherMock` em `Application/Doacoes/EnviarIntencao/Mocks/` segue convenção `Setup*` fluente + `Verify*` void.

---

## ✅ Fase 7 — Consumer `DoacaoProcessadaEvent` + policy de meta (concluída)

**Status:** ✔️ Concluída · 124 testes passando

**Entregues:**
- `src/Esperanca.Campanha.Domain/Doacoes/ArrecadacaoProcessada.cs` — entidade simples (PK `IdDoacao`) usada como ledger de idempotência defensiva.
- `src/Esperanca.Campanha.Application/Doacoes/_Shared/Contracts/DoacaoProcessadaEvent.cs` — contrato consumido pelo serviço (record).
- `src/Esperanca.Campanha.Application/Doacoes/ProcessarDoacaoProcessada/{ProcessarDoacaoProcessadaCommand,Handler}.cs` — handler verifica `arrecadacoes_processadas` (idempotência), carrega `Campanha`, chama `RegistrarArrecadacao(Valor)` e, se `PodeConcluirPorMeta()`, `ConcluirPorMeta()`. Persiste tudo em um único `SaveChangesAsync`. Campanha inexistente é ignorada com warning.
- `src/Esperanca.Campanha.Infrastructure/Doacoes/Persistence/ArrecadacaoProcessadaConfiguration.cs` — mapping EF (PK `IdDoacao`, `numeric(18,2)` para Valor, índice em `IdCampanha`).
- `src/Esperanca.Campanha.Infrastructure/Migrations/20260508211210_ArrecadacoesProcessadas.cs` — migration que cria tabela `arrecadacoes_processadas`.
- `src/Esperanca.Campanha.Infrastructure/Doacoes/RabbitMq/`:
  - `RabbitMqOptions` refatorado: chaves `RecebidaQueue/RecebidaRoutingKey/RecebidaDeadLetterQueue`, `ProcessadaQueue/ProcessadaRoutingKey/ProcessadaDeadLetterQueue`, `PrefetchCount`. `appsettings.json` atualizado.
  - `RabbitMqTopology` (helper) declara `DeadLetter` (fanout) e `WorkQueue` (direct com DLX) — usado por publisher e consumer.
  - `RabbitMqDoacaoPublisher` agora usa `RecebidaRoutingKey` e delega topologia ao helper.
  - `RabbitMqDoacaoProcessadaConsumer` (`BackgroundService`) — conecta ao broker, declara topologia idempotente para `doacoes-processadas` + DLQ `doacoes-processadas-dlq`, aplica `BasicQosAsync(prefetchCount=1)`, ACK manual após `IMediator.Send` (escopo por mensagem). Falhas de payload/handler → `BasicNackAsync(requeue=false)` → DLQ.
- `CampanhaInfrastructureModule` registra `RabbitMqDoacaoProcessadaConsumer` via `AddHostedService`.
- `test/.../Application/_Shared/Mocks/AppDbContextMock.cs` — adicionado `ArrecadacoesProcessadasDbSet` + `Setup`/`Verify` correspondentes.
- `test/.../Application/Doacoes/ProcessarDoacaoProcessada/`:
  - Handler: 5 cenários (sucesso registra arrecadação + idempotência; mensagem repetida não soma; valor ≥ meta dispara `Concluida`; modo `PorData` mantém `EmAndamento` mesmo com meta atingida; campanha inexistente é ignorada).
  - Contrato: 2 cenários (round-trip e leitura de payload externo).
- `CampanhaWebApplicationFactory` atualizada para remover `IHostedService` registrados (impede o consumer de tentar conectar ao broker durante smoke tests).

---

## ✅ Fase 8 — Scheduler (concluída)

**Status:** ✔️ Concluída · 130 testes passando

**Entregues:**
- `src/Esperanca.Campanha.Application/Campanhas/EncerrarVencidas/{EncerrarCampanhasVencidasCommand,Handler}.cs` — handler MediatR puro: itera `Status == EmAndamento`, chama `ConcluirPorData(agora)` quando `PodeConcluirPorData`, loga `CampanhaProximaDoVencimento` (nível Information, com `IdCampanha`/`DataFim`/`ValorArrecadado`/`MetaFinanceira`) para campanhas dentro da janela `ProximidadeVencimentoEmDias`. `SaveChangesAsync` só é disparado quando há ao menos uma conclusão.
- `src/Esperanca.Campanha.Infrastructure/Campanhas/Scheduler/SchedulerOptions.cs` — `IntervaloEmSegundos` (default 60), `ProximidadeVencimentoEmDias` (default 3).
- `src/Esperanca.Campanha.Infrastructure/Campanhas/Scheduler/CampanhaSchedulerService.cs` — `BackgroundService` que executa `IMediator.Send(EncerrarCampanhasVencidasCommand(...))` em escopo próprio a cada tick. Erros de tick são logados e não derrubam o loop.
- `CampanhaInfrastructureModule` registra `SchedulerOptions` (binda `Scheduler:*`) e o `CampanhaSchedulerService` como hosted service. Smoke tests continuam imunes (factory já remove `IHostedService`).
- `appsettings.json` ganhou seção `Scheduler:*`.
- `test/.../Application/Campanhas/EncerrarVencidas/EncerrarCampanhasVencidasHandlerTest.cs` — 6 cenários: encerra `PorData` vencida; **não** encerra `PorMeta` vencida sem meta atingida; campanhas próximas/distantes não persistem; sem campanhas `EmAndamento` não persiste; mistura de cenários processada corretamente em uma única passagem.

---

## ✅ Fase 9 — Transparência (MongoDB read side) (concluída)

**Status:** ✔️ Concluída · 136 testes passando

**Entregues:**
- `src/Esperanca.Campanha.Application/Transparencia/_Shared/`:
  - DTOs: `PainelMacroDto`, `TopDoadorDto`, `CampanhaTransparenciaDto`, `CampanhaDetalheDto`, `DoacaoAnonimaDto`.
  - `ITransparenciaReadRepository` com 3 métodos: `ObterPainelMacroAsync`, `ListarCampanhasAsync`, `ObterDetalheCampanhaAsync`.
- 3 slices (`Application/Transparencia/`):
  - `ConsultarPainelMacro/{Query,Handler}.cs` — quando o read model está vazio devolve um painel zerado (não falha) para dar smoke pública mesmo sem o Worker.
  - `ConsultarListaCampanhas/{Query,Handler}.cs` — devolve a lista (já vem ordenada `EmAndamento` → `Concluida`, depois por `DataInicio desc` no repository).
  - `ConsultarDetalheCampanha/{Query,Handler}.cs` — 404 (`Campanha:901`) quando o read model não tem o documento.
- `src/Esperanca.Campanha.WebApi/Transparencia/TransparenciaController.cs` — `[AllowAnonymous]` com `GET /api/transparencia/painel`, `GET /api/transparencia/campanhas` e `GET /api/transparencia/campanhas/{id:guid}`.
- `src/Esperanca.Campanha.Infrastructure/Transparencia/Mongo/`:
  - `TransparenciaMongoOptions` (DatabaseName + 3 nomes de collection).
  - `Documents.cs` — POCOs com `[BsonElement]` para mapear `painel_macro`, `lista_campanhas`, `campanha_detalhe`.
  - `TransparenciaMongoRepository` (escopo Scoped) lê via `IMongoClient` registrado como Singleton.
- `CampanhaInfrastructureModule` registra `IMongoClient` (`ConnectionStrings:DoacoesMongo`), `TransparenciaMongoOptions` e `ITransparenciaReadRepository → TransparenciaMongoRepository`.
- `Directory.Packages.props` + `Infrastructure.csproj`: `MongoDB.Driver 3.8.0`.
- `appsettings.json`: novas chaves `ConnectionStrings:DoacoesMongo` e `TransparenciaMongo:*`.
- `docker-compose.yml`: serviço `mongo:7` com healthcheck e `./docker/mongo/init-mongo.js` montado em `/docker-entrypoint-initdb.d`. `campanha-api` agora depende de `mongo` (healthy) e recebe `ConnectionStrings__DoacoesMongo`.
- `docker/mongo/init-mongo.js`: seed de `painel_macro`, `lista_campanhas`, `campanha_detalhe` com 2 campanhas mock para smoke local sem o Worker.
- `test/.../Application/Transparencia/`:
  - `_Shared/Mocks/TransparenciaReadRepositoryMock.cs` (Setup fluente para cada um dos 3 métodos).
  - `_Shared/Fakers/TransparenciaFaker.cs` (DTO factories).
  - `ConsultarPainelMacro/` — 2 cenários (painel real do Mongo; Mongo vazio devolve painel zerado).
  - `ConsultarListaCampanhas/` — 2 cenários (com campanhas; lista vazia).
  - `ConsultarDetalheCampanha/` — 2 cenários (encontrado; 404).

---

## ⏭️ Fase 10 — Cross-cutting

- [ ] `/health` agregando Postgres + Mongo + RabbitMQ (`Microsoft.Extensions.Diagnostics.HealthChecks`).
- [ ] Serilog estruturado + `correlation-id` propagado em headers RabbitMQ.
- [ ] Swagger com botão Bearer.
- [ ] `docker-compose.yml` (Postgres + Mongo + RabbitMQ + API) revisado e README de execução.

---

## Mapa rápido — onde paramos

- **Última fase concluída:** Fase 9 (Transparência — read side MongoDB, 3 endpoints públicos, seed mock).
- **Próximo passo:** Fase 10 (Cross-cutting — `/health` agregando Postgres+Mongo+RabbitMQ, Serilog estruturado com correlation-id, Swagger com botão Bearer, README de execução).
- **Para retomar:** rode `dotnet test test/Esperanca.Campanha.UnitTests` para confirmar baseline verde (136 testes), depois siga a checklist da Fase 10.
