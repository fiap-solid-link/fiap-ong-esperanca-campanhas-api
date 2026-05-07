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

## ⏭️ Fase 2 — Abstrações Application (próxima)

- [ ] `DbSet<Campanha>` em `IAppDbContext`.
- [ ] `IDateTimeProvider` (Application) + `SystemDateTimeProvider` (Infra).
- [ ] `ICurrentUser` (`UserId`, `Roles`, `EstaNaRole(string)`) — implementação Infra extrai de `HttpContext.User`.
- [ ] `IEntityTypeConfiguration<Campanha>` na Infra (mapping + index `IdGestor`/`Status`).
- [ ] Códigos novos em `CampanhaErrorCodes` (Application) + textos em `Localization/{pt-BR,en}.json`.

**Testes:** unit do `SystemDateTimeProvider` (UTC), unit do `CurrentUser` extraindo claim `sub`/`roles`.

---

## ⏭️ Fase 3 — Slices Write (CRUD da Campanha)

Cada slice = `Command` + `Handler` + `Validator` + DTO + testes (handler com EF InMemory + validator com FluentValidation.TestHelper).

- [ ] `CriarCampanha` — POST `/api/campanhas`.
- [ ] `EditarCampanha` — PUT `/api/campanhas/{id}` (só `Cadastrada`).
- [ ] `AtivarCampanha` — POST `/.../ativar`.
- [ ] `ProrrogarCampanha` — POST `/.../prorrogar`.
- [ ] `CancelarCampanha` — POST `/.../cancelar`.

**Autorização:** `[Authorize(Roles = "GestorONG")]` no controller + checagem `IdGestor == currentUser.UserId` no handler (404 se outra pessoa).

---

## ⏭️ Fase 4 — Slices Read (gestão)

- [ ] `ObterCampanha` — GET `/api/campanhas/{id}` (escopo do gestor).
- [ ] `ListarCampanhasGestor` — GET `/api/campanhas` com paginação + filtros (status, intervalo de datas).

---

## ⏭️ Fase 5 — Infrastructure write side + Controller + JWT

- [ ] Migration EF Core inicial para `Campanha`.
- [ ] `CampanhaController` finíssimo.
- [ ] JWT HMAC-SHA256 via `Jwt:*` (mesma chave do `identity-api`).
- [ ] Composition root: `CampanhaWebApiModule.ConfigureServices` no `Program.cs`.
- [ ] Smoke `WebApplicationFactory`: 201 com role correto, 401 sem token, 403 com role errado.

---

## ⏭️ Fase 6 — Doações (intenção, síncrona)

- [ ] Contrato `DoacaoRecebidaEvent` (record) em `Application/Doacoes/Contracts`.
- [ ] Abstração `IDoacaoPublisher`.
- [ ] Slice `EnviarIntencaoDoacao`:
  - Valida campanha existe + `EmAndamento` + `Valor > 0`.
  - Pega `IdDoador` do `ICurrentUser`, gera `IdDoacao` e `IdempotencyKey` (Guid).
  - Publica via `IDoacaoPublisher` → `202 Accepted`.
- [ ] `DoacaoController` com `[Authorize(Roles = "Doador,GestorONG")]`.
- [ ] **Infra**: `RabbitMqDoacaoPublisher` + bootstrap topologia (exchange `esperanca.doacoes` direct, fila `doacoes-recebidas` com DLX `esperanca.doacoes.dlx` → `doacoes-recebidas-dlq`).
- [ ] Config `RabbitMq:*`.
- [ ] **Teste de contrato**: serializar/desserializar `DoacaoRecebidaEvent` preservando todos os campos.

---

## ⏭️ Fase 7 — Consumer `DoacaoProcessadaEvent` + policy de meta

> Commits `feb2881` (consumer) e referências de scheduler trazem código da estrutura antiga — **portar**.

- [ ] Consumer (Infra `BackgroundService`) com prefetch=1, ACK manual.
- [ ] Handler interno: carrega `Campanha`, chama `RegistrarArrecadacao(Valor)` e, se `PodeConcluirPorMeta()`, `ConcluirPorMeta()`.
- [ ] **Idempotência defensiva**: tabela `arrecadacoes_processadas (id_doacao PK)` antes de aplicar — protege reentrância. Coberta por teste.

**Teste-chave:** mensagem repetida não soma duas vezes; valor que ultrapassa meta dispara `Concluida`.

---

## ⏭️ Fase 8 — Scheduler

> Commit `30a0037` (scheduler) + testes em `e19d28f` — portar.

- [ ] `CampanhaSchedulerService` (BackgroundService).
- [ ] Config: `Scheduler:IntervaloEmSegundos` (default 60), `Scheduler:ProximidadeVencimentoEmDias` (default 3).
- [ ] Por tick: para cada `EmAndamento` → `ConcluirPorData` se aplicável; log estruturado `CampanhaProximaDoVencimento` na janela.

---

## ⏭️ Fase 9 — Transparência (MongoDB read side)

- [ ] `ITransparenciaReadRepository` (Application) + 3 queries: `ConsultarPainelMacro`, `ConsultarListaCampanhas`, `ConsultarDetalheCampanha`.
- [ ] `TransparenciaMongoRepository` (Infra) lê `painel_macro`, `lista_campanhas`, `campanha_detalhe`.
- [ ] `TransparenciaController` com `[AllowAnonymous]`.
- [ ] Config `ConnectionStrings:DoacoesMongo`.
- [ ] **Seed**: `init-mongo.js` no `docker-compose.yml` para smoke local sem o Worker.

---

## ⏭️ Fase 10 — Cross-cutting

- [ ] `/health` agregando Postgres + Mongo + RabbitMQ (`Microsoft.Extensions.Diagnostics.HealthChecks`).
- [ ] Serilog estruturado + `correlation-id` propagado em headers RabbitMQ.
- [ ] Swagger com botão Bearer.
- [ ] `docker-compose.yml` (Postgres + Mongo + RabbitMQ + API) revisado e README de execução.

---

## Mapa rápido — onde paramos

- **Última fase concluída:** Fase 1 (Domain).
- **Próximo passo:** Fase 2 (abstrações da Application + `IDateTimeProvider` + `ICurrentUser` + mapping EF).
- **Para retomar:** rode `dotnet test test/Esperanca.Campanha.UnitTests` para confirmar baseline verde, depois siga a checklist da Fase 2.
