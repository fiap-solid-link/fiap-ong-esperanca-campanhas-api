using Esperanca.Campanha.Application.Transparencia._Shared;
using Esperanca.Campanha.Application.Transparencia.ConsultarListaCampanhas;
using Esperanca.Campanha.UnitTests.Application.Transparencia._Shared.Fakers;
using Esperanca.Campanha.UnitTests.Application.Transparencia.ConsultarListaCampanhas.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Transparencia.ConsultarListaCampanhas;

public class ConsultarListaCampanhasHandlerTest
{
    [Fact]
    public async Task Handle_WhenHaCampanhas_ThenRetornaTodas()
    {
        // Arrange
        var fixture = new ConsultarListaCampanhasHandlerFixture();
        IReadOnlyList<CampanhaTransparenciaDto> campanhas =
        [
            TransparenciaFaker.CampanhaTransparencia(status: "EmAndamento", titulo: "Campanha A"),
            TransparenciaFaker.CampanhaTransparencia(status: "Concluida",   titulo: "Campanha B"),
        ];
        fixture.RepositoryMock.SetupListaCampanhas(campanhas);

        // Act
        var result = await fixture.Handler.Handle(new ConsultarListaCampanhasQuery(), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.StatusCode.ShouldBe(200);
        result.Dados.ShouldBe(campanhas);
    }

    [Fact]
    public async Task Handle_WhenSemCampanhas_ThenRetornaListaVazia()
    {
        // Arrange
        var fixture = new ConsultarListaCampanhasHandlerFixture();
        fixture.RepositoryMock.SetupListaCampanhas([]);

        // Act
        var result = await fixture.Handler.Handle(new ConsultarListaCampanhasQuery(), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.Dados.ShouldNotBeNull();
        result.Dados.ShouldBeEmpty();
    }
}
