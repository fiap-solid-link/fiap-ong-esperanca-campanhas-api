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

- **Última fase concluída:** Fase 3 (slices Write — CriarCampanha, EditarCampanha, AtivarCampanha, ProrrogarCampanha, CancelarCampanha).
- **Próximo passo:** Fase 4 (slices Read — ObterCampanha, ListarCampanhasGestor).
- **Para retomar:** rode `dotnet test test/Esperanca.Campanha.UnitTests` para confirmar baseline verde (89 testes), depois siga a checklist da Fase 4.
