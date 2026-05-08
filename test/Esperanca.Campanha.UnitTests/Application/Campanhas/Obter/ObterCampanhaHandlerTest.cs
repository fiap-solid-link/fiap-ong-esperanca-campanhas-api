using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Campanhas.Obter;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Obter.Fakers;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Obter.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Obter;

public class ObterCampanhaHandlerTest
{
    [Fact]
    public async Task Handle_WhenCampanhaExisteEPertenceAoGestor_ThenReturnOkWithDto()
    {
        // Arrange
        var fixture = new ObterCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmCadastrada();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);

        // Act
        var result = await fixture.Handler.Handle(new ObterCampanhaQuery(campanha.Id), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.StatusCode.ShouldBe(200);
        result.Dados.ShouldNotBeNull();
        result.Dados.Id.ShouldBe(campanha.Id);
        result.Dados.Titulo.ShouldBe(campanha.Titulo);
        result.Dados.IdGestor.ShouldBe(campanha.IdGestor);
    }

    [Fact]
    public async Task Handle_WhenCampanhaNaoExiste_ThenReturnNotFound()
    {
        // Arrange
        var fixture = new ObterCampanhaHandlerFixture();
        fixture.AppDbContextMock.SetupCampanhas([]);

        // Act
        var result = await fixture.Handler.Handle(new ObterCampanhaQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(404);
        result.Erro.ShouldBe(CampanhaErrorCodes.CampanhaNaoEncontrada);
        result.Dados.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_WhenCampanhaPertenceAOutroGestor_ThenReturnNotFound()
    {
        // Arrange
        var fixture = new ObterCampanhaHandlerFixture();
        var campanha = CampanhaFaker.DeOutroGestor();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);

        // Act
        var result = await fixture.Handler.Handle(new ObterCampanhaQuery(campanha.Id), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(404);
        result.Erro.ShouldBe(CampanhaErrorCodes.CampanhaNaoEncontrada);
        result.Dados.ShouldBeNull();
    }
}
