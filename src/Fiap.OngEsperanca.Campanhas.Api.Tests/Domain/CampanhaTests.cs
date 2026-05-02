using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Fiap.OngEsperanca.Campanhas.Api.Tests.Domain;

public class CampanhaTests
{
    [Fact(DisplayName = "Deve somar o valor arrecadado corretamente ao receber uma doação")]
    public void AdicionarArrecadacao_ValorValido_DeveSomarNoTotal()
    {
        // Arrange: Preparamos uma campanha zerada
        var campanha = new Campanha("Campanha de Inverno", "Agasalhos", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), 1000m);

        // Act: Simulamos a chegada de R$ 150,00
        campanha.AdicionarArrecadacao(150m);

        // Assert: O valor total deve ser exatamente 150
        campanha.ValorTotalArrecadado.Should().Be(150m);
    }

    [Fact(DisplayName = "Deve somar multiplas arrecadacoes acumulando o valor")]
    public void AdicionarArrecadacao_MultiplasDoacoes_DeveAcumular()
    {
        // Arrange
        var campanha = new Campanha("Campanha de Inverno", "Agasalhos", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), 1000m);

        // Act
        campanha.AdicionarArrecadacao(50m);
        campanha.AdicionarArrecadacao(150m);

        // Assert: 50 + 150 = 200
        campanha.ValorTotalArrecadado.Should().Be(200m);
    }
}