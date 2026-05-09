using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Transparencia.ConsultarDetalheCampanha;
using Esperanca.Campanha.UnitTests.Application.Transparencia._Shared.Fakers;
using Esperanca.Campanha.UnitTests.Application.Transparencia.ConsultarDetalheCampanha.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Transparencia.ConsultarDetalheCampanha;

public class ConsultarDetalheCampanhaHandlerTest
{
    [Fact]
    public async Task Handle_WhenCampanhaExiste_ThenRetornaDetalhe()
    {
        // Arrange
        var fixture = new ConsultarDetalheCampanhaHandlerFixture();
        var idCampanha = Guid.NewGuid();
        var detalhe = TransparenciaFaker.DetalheCampanha(idCampanha);
        fixture.RepositoryMock.SetupDetalheCampanha(idCampanha, detalhe);

        // Act
        var result = await fixture.Handler.Handle(new ConsultarDetalheCampanhaQuery(idCampanha), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.StatusCode.ShouldBe(200);
        result.Dados.ShouldBe(detalhe);
        result.Dados!.Doacoes.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_WhenCampanhaNaoExiste_ThenRetornaNotFound()
    {
        // Arrange
        var fixture = new ConsultarDetalheCampanhaHandlerFixture();
        var idCampanha = Guid.NewGuid();
        fixture.RepositoryMock.SetupDetalheCampanha(idCampanha, null);

        // Act
        var result = await fixture.Handler.Handle(new ConsultarDetalheCampanhaQuery(idCampanha), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(404);
        result.Erro.ShouldBe(CampanhaErrorCodes.CampanhaNaoEncontrada);
        result.Dados.ShouldBeNull();
    }
}
