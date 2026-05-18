using System.Text;
using System.Text.Json;
using Esperanca.Campanha.Infrastructure.Doacoes.RabbitMq;
using Esperanca.Message.Events;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Infrastructure.Doacoes.RabbitMq;

public class RabbitMqDoacaoProcessadaConsumerTest
{
    [Fact]
    public void CriarCommand_WhenPayloadValido_ThenMapeiaTodosOsCampos()
    {
        // Arrange
        var evento = new DoacaoProcessadaEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            100m,
            1500m,
            new DateTime(2026, 5, 17, 23, 9, 53, DateTimeKind.Utc));

        var payload = JsonSerializer.SerializeToUtf8Bytes(evento);

        // Act
        var command = RabbitMqDoacaoProcessadaConsumer.CriarCommand(payload);

        // Assert
        command.IdDoacao.ShouldBe(evento.IdDoacao);
        command.IdCampanha.ShouldBe(evento.IdCampanha);
        command.Valor.ShouldBe(evento.Valor);
        command.ValorTotalArrecadado.ShouldBe(evento.ValorTotalArrecadado);
        command.DataProcessamento.ShouldBe(evento.DataProcessamento);
    }

    [Fact]
    public void CriarCommand_WhenPayloadInvalido_ThenThrowsJsonException()
    {
        // Arrange
        var payload = Encoding.UTF8.GetBytes("{ payload-invalido }");

        // Act
        var act = () => RabbitMqDoacaoProcessadaConsumer.CriarCommand(payload);

        // Assert
        Should.Throw<JsonException>(act);
    }

    [Fact]
    public void ExtractCorrelationId_WhenHeaderExiste_ThenRetornaValorDoHeader()
    {
        // Arrange
        var headers = new Dictionary<string, object?>
        {
            ["X-Correlation-Id"] = Encoding.UTF8.GetBytes("correlation-test")
        };

        var correlationId = RabbitMqDoacaoProcessadaConsumer.ExtractCorrelationId(headers);

        correlationId.ShouldBe("correlation-test");

        // Act
        var result = RabbitMqDoacaoProcessadaConsumer.ExtractCorrelationId(headers);

        // Assert
        result.ShouldBe(correlationId);
    }

    [Fact]
    public void ExtractCorrelationId_WhenHeaderNaoExiste_ThenGeraCorrelationId()
    {
        // Act
        var result = RabbitMqDoacaoProcessadaConsumer.ExtractCorrelationId(null);

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        Guid.TryParseExact(result, "N", out _).ShouldBeTrue();
    }
}
