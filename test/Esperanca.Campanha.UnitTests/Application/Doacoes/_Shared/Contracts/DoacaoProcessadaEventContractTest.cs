using System.Text.Json;
using Esperanca.Message.Events;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes._Shared.Contracts;

public class DoacaoProcessadaEventContractTest
{
    private static readonly DoacaoProcessadaEvent Sample = new(
        IdDoacao:          Guid.Parse("11111111-1111-1111-1111-111111111111"),
        IdCampanha:        Guid.Parse("22222222-2222-2222-2222-222222222222"),
        Valor:             123.45m,
        ValorTotalArrecadado:123.45m,
        DataProcessamento: new DateTime(2026, 5, 8, 13, 0, 0, DateTimeKind.Utc));

    [Fact]
    public void RoundTrip_PreservaTodosOsCampos()
    {
        // Act
        var json = JsonSerializer.Serialize(Sample);
        var deserialized = JsonSerializer.Deserialize<DoacaoProcessadaEvent>(json);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.IdDoacao.ShouldBe(Sample.IdDoacao);
        deserialized.IdCampanha.ShouldBe(Sample.IdCampanha);
        deserialized.Valor.ShouldBe(Sample.Valor);
        deserialized.ValorTotalArrecadado.ShouldBe(Sample.ValorTotalArrecadado);
        deserialized.DataProcessamento.ShouldBe(Sample.DataProcessamento);
    }

    [Fact]
    public void Desserializa_AceitaPayloadProduzidoPeloWorker()
    {
        // Arrange — payload "vindo do worker" (mesmo schema, cultura invariante)
        const string jsonExterno = """
            {
              "IdDoacao": "11111111-1111-1111-1111-111111111111",
              "IdCampanha": "22222222-2222-2222-2222-222222222222",
              "Valor": 123.45,
              "ValorTotalArrecadado": 123.45,
              "DataProcessamento": "2026-05-08T13:00:00Z"
            }
            """;

        // Act
        var deserialized = JsonSerializer.Deserialize<DoacaoProcessadaEvent>(jsonExterno);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.IdDoacao.ShouldBe(Sample.IdDoacao);
        deserialized.IdCampanha.ShouldBe(Sample.IdCampanha);
        deserialized.Valor.ShouldBe(Sample.Valor);
        deserialized.ValorTotalArrecadado.ShouldBe(Sample.ValorTotalArrecadado);
        deserialized.DataProcessamento.ToUniversalTime().ShouldBe(Sample.DataProcessamento);
    }
}
