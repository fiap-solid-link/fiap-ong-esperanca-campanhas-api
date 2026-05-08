using System.Text.Json;
using Esperanca.Campanha.Application.Doacoes._Shared.Contracts;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes._Shared.Contracts;

public class DoacaoRecebidaEventContractTest
{
    private static readonly DoacaoRecebidaEvent Sample = new(
        IdDoacao:       Guid.Parse("11111111-1111-1111-1111-111111111111"),
        IdCampanha:     Guid.Parse("22222222-2222-2222-2222-222222222222"),
        IdDoador:       Guid.Parse("33333333-3333-3333-3333-333333333333"),
        Valor:          123.45m,
        DataIntencao:   new DateTime(2026, 5, 8, 12, 30, 0, DateTimeKind.Utc),
        IdempotencyKey: Guid.Parse("44444444-4444-4444-4444-444444444444"));

    [Fact]
    public void RoundTrip_PreservaTodosOsCampos()
    {
        // Act
        var json = JsonSerializer.Serialize(Sample);
        var deserialized = JsonSerializer.Deserialize<DoacaoRecebidaEvent>(json);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.IdDoacao.ShouldBe(Sample.IdDoacao);
        deserialized.IdCampanha.ShouldBe(Sample.IdCampanha);
        deserialized.IdDoador.ShouldBe(Sample.IdDoador);
        deserialized.Valor.ShouldBe(Sample.Valor);
        deserialized.DataIntencao.ShouldBe(Sample.DataIntencao);
        deserialized.IdempotencyKey.ShouldBe(Sample.IdempotencyKey);
    }

    [Fact]
    public void Serializa_ContemTodasAsPropriedadesEsperadas()
    {
        // Act
        var json = JsonSerializer.Serialize(Sample);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Assert
        root.TryGetProperty("IdDoacao", out _).ShouldBeTrue();
        root.TryGetProperty("IdCampanha", out _).ShouldBeTrue();
        root.TryGetProperty("IdDoador", out _).ShouldBeTrue();
        root.TryGetProperty("Valor", out _).ShouldBeTrue();
        root.TryGetProperty("DataIntencao", out _).ShouldBeTrue();
        root.TryGetProperty("IdempotencyKey", out _).ShouldBeTrue();
    }

    [Fact]
    public void Desserializa_AceitaPayloadProduzidoPorOutroService()
    {
        // Arrange — payload "vindo do worker" (mesmo schema, cultura invariante)
        const string jsonExterno = """
            {
              "IdDoacao": "11111111-1111-1111-1111-111111111111",
              "IdCampanha": "22222222-2222-2222-2222-222222222222",
              "IdDoador": "33333333-3333-3333-3333-333333333333",
              "Valor": 123.45,
              "DataIntencao": "2026-05-08T12:30:00Z",
              "IdempotencyKey": "44444444-4444-4444-4444-444444444444"
            }
            """;

        // Act
        var deserialized = JsonSerializer.Deserialize<DoacaoRecebidaEvent>(jsonExterno);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.IdDoacao.ShouldBe(Sample.IdDoacao);
        deserialized.IdCampanha.ShouldBe(Sample.IdCampanha);
        deserialized.IdDoador.ShouldBe(Sample.IdDoador);
        deserialized.Valor.ShouldBe(Sample.Valor);
        deserialized.DataIntencao.ToUniversalTime().ShouldBe(Sample.DataIntencao);
        deserialized.IdempotencyKey.ShouldBe(Sample.IdempotencyKey);
    }
}
