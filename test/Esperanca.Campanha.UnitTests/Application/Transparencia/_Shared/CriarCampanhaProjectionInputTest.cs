using Esperanca.Campanha.Application.Transparencia._Shared;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Transparencia._Shared;

public class CriarCampanhaProjectionInputTest
{
    [Fact]
    public void Constructor_WhenCalled_ThenPreservesAllValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dataInicio = new DateTime(2026, 5, 1);
        var dataFim = new DateTime(2026, 5, 30);
        var dataEncerramento = new DateTime(2026, 5, 20);

        // Act
        var input = new CriarCampanhaProjectionInput(
            id,
            "Campanha",
            "Descrição",
            1000m,
            10m,
            "Cadastrada",
            dataInicio,
            dataFim,
            dataEncerramento);

        // Assert
        input.IdCampanha.ShouldBe(id);
        input.Titulo.ShouldBe("Campanha");
        input.Descricao.ShouldBe("Descrição");
        input.MetaFinanceira.ShouldBe(1000m);
        input.ValorArrecadado.ShouldBe(10m);
        input.Status.ShouldBe("Cadastrada");
        input.DataInicio.ShouldBe(dataInicio);
        input.DataFim.ShouldBe(dataFim);
        input.DataEncerramento.ShouldBe(dataEncerramento);
    }

    [Fact]
    public void Equality_WhenValuesAreEqual_ThenRecordsAreEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dataInicio = new DateTime(2026, 5, 1);
        var dataFim = new DateTime(2026, 5, 30);

        var input1 = new CriarCampanhaProjectionInput(id, "A", "B", 100m, 0m, "Cadastrada", dataInicio, dataFim, null);
        var input2 = new CriarCampanhaProjectionInput(id, "A", "B", 100m, 0m, "Cadastrada", dataInicio, dataFim, null);

        // Assert
        input1.ShouldBe(input2);
    }
}
