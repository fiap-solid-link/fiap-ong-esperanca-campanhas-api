# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Visão geral

API ASP.NET Core (.NET 10) do contexto **Campanhas** da plataforma "Conexão Solidária" (ONG Esperança / FIAP). Expõe endpoints para criar, listar, editar, ativar, prorrogar e cancelar campanhas, além de receber intenções de doação. Persistência em PostgreSQL via EF Core; mensageria via RabbitMQ (consumer de `DoacaoProcessada` para atualizar arrecadação); scheduler em background para encerramento automático de campanhas vencidas.

## Comandos comuns

Solution: `Esperanca.Campanha.sln`. Sempre operar a partir da raiz do repo.

```powershell
# Restore + build da solution inteira
dotnet build Esperanca.Campanha.sln

# Subir Postgres + API em containers (porta 5010 -> 8080)
docker compose up -d --build

# Rodar a API localmente (precisa de Postgres ouvindo em localhost:5432)
dotnet run --project src/Esperanca.Campanha.WebApi

# Testes
dotnet test                                          # toda a solution
dotnet test test/Esperanca.Campanha.UnitTests        # só o projeto de testes
dotnet test --filter "FullyQualifiedName~CriarCampanha"   # filtra por nome
dotnet test --filter "DisplayName=Nome.Do.Teste"          # roda um teste específico

# Migrations EF Core (DbContext fica em Infrastructure, mas startup é o WebApi)
dotnet ef migrations add <Nome> -p src/Esperanca.Campanha.Infrastructure -s src/Esperanca.Campanha.WebApi
dotnet ef database update     -p src/Esperanca.Campanha.Infrastructure -s src/Esperanca.Campanha.WebApi
```

Endpoints úteis quando a API está rodando: `GET /swagger`, `GET /health`.

## Arquitetura (Clean Architecture / 4 camadas)

```
src/
  Esperanca.Campanha.Domain          (entidades + regras puras)
  Esperanca.Campanha.Application     (casos de uso, MediatR, FluentValidation)
  Esperanca.Campanha.Infrastructure  (EF Core / Postgres, integrações externas)
  Esperanca.Campanha.WebApi          (Controllers, middleware, composition root)
test/
  Esperanca.Campanha.UnitTests       (xUnit + NSubstitute)
```

Direção de dependências: `WebApi → Application + Infrastructure`, `Infrastructure → Application`, `Application → Domain`. Nada depende de `WebApi` ou `Infrastructure` no sentido inverso. **A camada Application define interfaces (ex.: `IAppDbContext`, `IAppLocalizer`); Infrastructure implementa.**

### Padrões fundamentais

- **Composition root via módulos**: cada camada expõe `static class XxxModule.ConfigureServices(...)`. `Program.cs` chama apenas `CampanhaWebApiModule`, que encadeia os módulos de Application e Infrastructure. Ao adicionar uma nova dependência, registre-a no módulo da camada onde ela vive — não no `Program.cs`.
- **CQRS com MediatR**: handlers (`IRequestHandler<TRequest, TResponse>`) ficam em `Application`. Controllers são finos e só fazem `mediator.Send(...)`.
- **Validação por pipeline**: `ValidationBehavior<TRequest,TResponse>` (`Application/_Shared/Behaviors`) roda todos os `IValidator<TRequest>` antes do handler e lança `FluentValidation.ValidationException` em falha. Não validar manualmente nos handlers.
- **Tradução de erros HTTP**: `ValidationExceptionMiddleware` (`WebApi/_Shared/Middleware`) captura `ValidationException` e devolve 400 com payload `{ titulo: "Campanha:900", erros: { ... } }`. Erros de domínio/aplicação fluem como `Result<T>`.
- **Result pattern**: `Application/_Shared/Results/Result<T>` é o tipo de retorno padrão dos handlers (`Ok`, `Created`, `Fail`, `NotFound`, `Unauthorized` + `StatusCode`). Controllers traduzem `Result.StatusCode` para `IActionResult`.
- **Acesso a dados**: handlers dependem de `IAppDbContext` (Application), nunca de `CampanhaDbContext` (Infrastructure). Configurações de entidade são descobertas via `ApplyConfigurationsFromAssembly` em `CampanhaDbContext.OnModelCreating` — adicione `IEntityTypeConfiguration<T>` no projeto Infrastructure para mapear novas entidades.
- **Localização / códigos de erro**: mensagens vivem em `Application/_Shared/Localization/{pt-BR,en}.json` indexadas por código (`Campanha:NNN`). Constantes em `CampanhaErrorCodes`. Resolva texto via `IAppLocalizer[code]`.
- **Convenção `_Shared`**: cada camada tem uma pasta `_Shared/` para plumbing transversal (behaviors, middleware, abstrações, result). Código específico de feature deve ficar agrupado por feature, não despejado em `_Shared`.

### Pacotes & versões

Versionamento NuGet é **centralizado**: `Directory.Packages.props` declara todas as versões; csprojs só referenciam `<PackageReference Include="..." />` sem `Version`. Para subir uma dependência, edite o `Directory.Packages.props`. Configurações comuns (`TargetFramework=net10.0`, `Nullable=enable`, `ImplicitUsings=enable`) ficam em `Directory.Build.props` e valem para todos os projetos.

### Configuração / segredos

- Connection string: `ConnectionStrings:CampanhaDb` (Postgres). Override em container via env `ConnectionStrings__CampanhaDb`.
- JWT: `Jwt:SecretKey/Issuer/Audience/AccessTokenExpirationMinutes`. O valor em `appsettings.json` é apenas para dev — substituir em produção.
- Logging: Serilog configurado por `appsettings.json` (sink Console por padrão).

## Estado atual do repo (importante)

O commit mais recente (`aa59632 feat: cria estrutura clean arc`) **renomeou a estrutura** de `src/Fiap.OngEsperanca.Campanhas.Api*` para os 4 projetos `Esperanca.Campanha.*`. Como consequência:

- O projeto **Domain** ainda contém apenas `Class1.cs` (placeholder). A entidade `Campanha`, handlers, validators, controllers e testes que existiam na estrutura antiga **ainda não foram portados** para o novo layout — ver commits anteriores (`beef411`, `7010dbc`, `feb2881`, `30a0037`, etc.) para a lógica original a ser reposicionada.
- Ao implementar uma feature, distribua o código nas camadas conforme os padrões acima — não recrie a pasta `Features/` plana da estrutura antiga dentro de um único projeto.
- O arquivo `Fiap.OngEsperanca.Campanhas.slnx` foi removido; use **`Esperanca.Campanha.sln`**.

## Testes

- Framework: xUnit; mocking: NSubstitute. `Using Include="Xunit"` é global no projeto de testes.
- O projeto de testes referencia Domain, Application **e WebApi** — testes podem cobrir desde regras de domínio até integração leve via `WebApplicationFactory` se necessário.
- Padrão observado nos commits anteriores: testes de handler usam EF Core InMemory para `IAppDbContext`; testes de validator exercitam `FluentValidation` diretamente.
