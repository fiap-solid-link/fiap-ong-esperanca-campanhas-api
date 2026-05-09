# Unit Testing

## Mandatory Coverage

Every change in **Domain** or **Application** requires unit tests covering **100% of paths**. No edge case can be left uncovered. Code without tests is not delivered code.

- Each conditional branch (`if`, `IsFailed`, null check) → dedicated test
- Each error path (`FailValidation`, `FailNotFound`, `Fail(ex)`) → dedicated test
- Each success scenario with relevant variations → dedicated test
- **Infrastructure layer is excluded** — if you need to test it, business logic has leaked

## Stack

| Concern | Tool |
|---------|------|
| Framework | xUnit |
| Mocking | NSubstitute |
| DbSet mocking | MockQueryable.NSubstitute (`BuildMockDbSet()`) |
| Assertions | Shouldly (`.ShouldBe()`, `.ShouldNotBeNull()`) |
| Test data | Static Faker classes (hardcoded, deterministic) or Bogus core (fixed seed) |

### Prohibited

- **AutoBogus** (automatic generation) — use Bogus **core** with fixed seed: `new Faker("pt_BR") { Random = new Randomizer(12345) }`
- **FluentAssertions** — use Shouldly
- Random data without seed — same input must yield same output, always

### Exception: `DateTime.UtcNow` in Fakers

In **production code** (Domain, Application, Infrastructure), prefer abstracting time via a clock interface. In **test Fakers**, `DateTime.UtcNow` is acceptable for building entities in specific states (e.g., campanha encerrada "5 minutos atrás"). This is not considered random data — the test verifies behavior, not exact timestamp values.

## Approach: Mockist (London School)

- Mock ALL external dependencies
- One test = one scenario
- Explicit AAA pattern (`// Arrange`, `// Act`, `// Assert`)
- Behavior verification via mock interaction checks

## Folder Structure

```
test/Esperanca.Campanha.UnitTests/
├── Domain/
│   └── {Feature}/
│       └── {ClassName}/
│           ├── Fakers/
│           │   └── {Entity}Faker.cs
│           └── {ClassName}Test.cs
├── Application/
│   └── {Feature}/
│       └── {OperationName}/
│           ├── Fixtures/
│           │   └── {ClassName}Fixture.cs
│           ├── Mocks/
│           │   └── {Dependency}Mock.cs
│           ├── Fakers/
│           │   └── {Entity}Faker.cs
│           └── {ClassName}Test.cs
└── WebApi/
    └── {Feature}/
        └── {OperationName}/
            ├── Fixtures/
            ├── Mocks/
            ├── Fakers/
            └── {ClassName}Test.cs
```

**Domain** has no `Mocks/` or `Fixtures/`.

## Naming

| Artifact | Pattern |
|----------|---------|
| Test class | `{ClassUnderTest}Test` |
| Test method | `{Method}_When{Condition}_Then{Result}` |
| Fixture class | `{ClassUnderTest}Fixture` |
| Mock class | `{Dependency}Mock` |
| Faker class | `{Entity}Faker` |
| Mock setup | `Setup{Action}Success()`, `Setup{Action}ReturnsNull()`, `Setup{Action}Throws()` |
| Mock verify | `Verify{Action}Called()`, `Verify{Action}NotCalled()` |
| Faker methods | `Valid()`, `WithDifferent{Variacao}()`, `WithInvalid{Field}()` |

Names in **English**. Domain entities keep their **Portuguese** names.

## Specialized Mock Classes

Application handlers depend on `IAppDbContext` (`Application/_Shared/IAppDbContext.cs`), which exposes `DbSet<T> Set<T>()` and `SaveChangesAsync(...)`. Tests mock the context and back each `Set<T>()` with `BuildMockDbSet()` from `MockQueryable.NSubstitute`.

Each dependency has a `{Dependency}Mock` using **composition** (not inheritance):

```csharp
using MockQueryable.NSubstitute;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Microsoft.EntityFrameworkCore;
using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Domain.Campanhas;

public class AppDbContextMock
{
    public IAppDbContext Instance { get; }
    public DbSet<Campanha> CampanhasDbSet { get; private set; } = default!;

    public AppDbContextMock()
    {
        Instance = Substitute.For<IAppDbContext>();
        SetupCampanhas([]);
    }

    // Setup — returns this for fluent chaining
    public AppDbContextMock SetupCampanhas(List<Campanha> data)
    {
        CampanhasDbSet = data.AsQueryable().BuildMockDbSet();
        Instance.Set<Campanha>().Returns(CampanhasDbSet);
        return this;
    }

    public AppDbContextMock SetupSaveChangesSuccess(int affected = 1)
    {
        Instance.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(affected);
        return this;
    }

    public AppDbContextMock SetupSaveChangesThrows(Exception exception)
    {
        Instance.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsync(exception);
        return this;
    }

    // Verify — void, encapsulate NSubstitute
    public void VerifyCampanhaAdded() =>
        CampanhasDbSet.Received(1).Add(Arg.Any<Campanha>());

    public void VerifyCampanhaNotAdded() =>
        CampanhasDbSet.DidNotReceive().Add(Arg.Any<Campanha>());

    public void VerifySaveChangesCalled() =>
        Instance.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

    public void VerifySaveChangesNotCalled() =>
        Instance.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
}
```

Rules:
- `Instance` — the substitute, created in the constructor
- One property per `DbSet<T>` exposed (`CampanhasDbSet`, etc.) so tests can `Verify*` on `Add`/`Update`/`Remove`
- `Setup*` — return `this` for fluent chaining
- `Verify*` — void, encapsulate the entire NSubstitute API
- `SetupSaveChangesThrows` — for testing the exception path in the handler

### Querying inside handlers (LINQ over DbSet)

Handlers query directly: `await _dbContext.Set<Campanha>().FirstOrDefaultAsync(c => c.Id == id, ct)`. `BuildMockDbSet()` returns a `DbSet<T>` that supports both sync and async LINQ over the in-memory list — `FirstOrDefaultAsync`, `ToListAsync`, `CountAsync` etc. all work without extra setup. Never manually implement `IAsyncQueryProvider`, `IAsyncEnumerator`, etc.

To exercise different data scenarios, call `SetupCampanhas(...)` per test (the constructor seeds an empty list as default).

## Fixture

Mandatory for **Application** and **WebApi**. Not used in **Domain**.

Plain class — no inheritance, no `IClassFixture<T>`:

```csharp
using Microsoft.Extensions.Logging;
using NSubstitute;
using Esperanca.Campanha.Application._Shared.Localization;

public class CriarCampanhaHandlerFixture
{
    public AppDbContextMock AppDbContextMock { get; }
    public CriarCampanhaHandler Handler { get; }

    public CriarCampanhaHandlerFixture()
    {
        AppDbContextMock = new AppDbContextMock();
        var logger       = Substitute.For<ILogger<CriarCampanhaHandler>>();
        var localizer    = Substitute.For<IAppLocalizer>();
        localizer[Arg.Any<string>()].Returns(c => c.Arg<string>());

        Handler = new CriarCampanhaHandler(
            logger,
            AppDbContextMock.Instance,
            localizer);
    }
}
```

Rules:
- Instantiates all specialized mocks
- Builds the SUT passing `.Instance` from each mock
- `ILogger` and `IAppLocalizer` are simple substitutes (no mock class). The localizer indexer returns the code itself, so assertions can check error codes (`Campanha:NNN`) without loading resource files
- **Instantiated per test** in `// Arrange`

## Fakers

Static classes with **deterministic data**. Same input = same output, always.

Two accepted approaches:

**Option 1 — Hardcoded** (literal data):

```csharp
public static CriarCampanhaCommand Valid()
{
    return new CriarCampanhaCommand("Nome", ...);
}
```

**Option 2 — Bogus with fixed seed** (deterministic via seed):

```csharp
private static readonly Faker _faker = new Faker("pt_BR") { Random = new Randomizer(12345) };

public static CriarCampanhaCommand Valid()
{
    return new CriarCampanhaCommand(_faker.Company.CompanyName());
}
```

**Consistency rule**: Within the same test module, prefer one style. If the module already uses hardcoded, continue with hardcoded.

### Application Fakers

```csharp
namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Criar.Fakers;

public static class CriarCampanhaCommandFaker
{
    public static CriarCampanhaCommand Valid()
    {
        return new CriarCampanhaCommand("Nome Campanha", 10000);
    }

    public static CriarCampanhaCommand WithDifferentMeta()
    {
        return new CriarCampanhaCommand("Nome Campanha", 50000);
    }
}
```

### Domain Fakers

Domain validates on creation and throws exceptions. `WithInvalid{Field}()` returns `Action` for `Should.Throw`:

```csharp
public static class CampanhaFaker
{
    public static Campanha Valid()
    {
        return new Campanha("Nome Campanha", 10000);
    }

    public static Action WithInvalidNome()
    {
        return () => new Campanha("", 10000);
    }

    public static Action WithInvalidMeta()
    {
        return () => new Campanha("Nome Campanha", -10000);
    }
}
```

### Faker Isolation

Each operation **has its own Fakers**, local to its folder. **Never share Fakers between layers or operations.** Duplication is expected and intentional.

## Assertions

Use **Shouldly** for all assertions. Mock verifications via `Verify*()` from mock classes.

```csharp
result.IsSuccess.ShouldBeTrue();
result.Value.Nome.ShouldBe(command.Nome);
result.IsFailed.ShouldBeTrue();
result.Value.ShouldNotBeNull();
fixture.AppDbContextMock.VerifyCampanhaAdded();
fixture.AppDbContextMock.VerifySaveChangesCalled();
fixture.AppDbContextMock.VerifyCampanhaNotAdded();
```

### Prohibited in Assertions

- FluentAssertions (`.Should()`, `.BeTrue()`, etc.)
- xUnit `Assert.*` — use Shouldly instead
- `Received()` or `DidNotReceive()` directly in the test (use `Verify*()` from mocks)

## Example — Command Handler Tests

AAA pattern with Fixture + Faker + Mock. A handler with success, business validation and persistence-exception paths → 3+ tests:

```csharp
[Fact]
public async Task Handle_WhenValidCommand_ThenReturnSuccessWithDto()
{
    // Arrange
    var fixture = new CriarCampanhaHandlerFixture();
    var command = CriarCampanhaCommandFaker.Valid();
    fixture.AppDbContextMock
        .SetupCampanhas([])
        .SetupSaveChangesSuccess();

    // Act
    var result = await fixture.Handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.Nome.ShouldBe(command.Nome);
    fixture.AppDbContextMock.VerifyCampanhaAdded();
    fixture.AppDbContextMock.VerifySaveChangesCalled();
}

[Fact]
public async Task Handle_WhenCampanhaJaExiste_ThenReturnFailedResult()
{
    // Arrange
    var fixture = new CriarCampanhaHandlerFixture();
    fixture.AppDbContextMock.SetupCampanhas([CampanhaFaker.Valid()]);

    // Act
    var result = await fixture.Handler.Handle(CriarCampanhaCommandFaker.Valid(), CancellationToken.None);

    // Assert
    result.IsFailed.ShouldBeTrue();
    fixture.AppDbContextMock.VerifyCampanhaNotAdded();
    fixture.AppDbContextMock.VerifySaveChangesNotCalled();
}
```

Pattern: **success** test (setup happy path + assert result + verify interactions) and **failure** test (setup error condition + assert `IsFailed` + verify that the next operation was **not** called). Repeat for each `catch` and each conditional branch.

## Rules per Layer

### Domain

- No mocks, no fixtures
- Tests business rules and invariants directly
- Each fluent setter with validation → success test + test per each validation
- Each mutation method → test of resulting state

### Application — Command Handlers

- Mock `IAppDbContext` and any other injected dependency (`IAppLocalizer`, `ILogger<T>`, message bus, scheduler clock, etc.)
- Verify mandatory interactions (`VerifyCalled` / `VerifyNotCalled`)
- Test: success, each business validation, each exception path

### Application — Query Handlers

- Mock `IAppDbContext` with `SetupCampanhas(data)` (which builds a `DbSet<T>` via `BuildMockDbSet()`)
- LINQ executes in memory, including async operators (`FirstOrDefaultAsync`, `ToListAsync`, `CountAsync`)
- Test: success with data, empty list, applied filters, exception

### WebApi

- Mock the mediator (`ISender` / `IMediator`) and assert that the controller forwards the right command/query
- Assert status codes and the `Result<T>.StatusCode` → `IActionResult` translation
- Cover `ValidationExceptionMiddleware` mapping (400 with `{ titulo: "Campanha:900", erros: { ... } }`) when relevant

## Prohibitions

- `Substitute.For<T>()` directly in the test (use Mock classes)
- `Received()`, `DidNotReceive()`, `Arg.Any<T>()` directly in the test (use Verify/Setup)
- Inline data creation in the test (use Fakers)
- Sharing Fakers between operations or layers
- Tests for the Infrastructure layer
- Multiple scenarios in a single test
- `Thread.Sleep`, order-dependent tests
- Manual implementation of `IAsyncQueryProvider` (use `BuildMockDbSet()`)
