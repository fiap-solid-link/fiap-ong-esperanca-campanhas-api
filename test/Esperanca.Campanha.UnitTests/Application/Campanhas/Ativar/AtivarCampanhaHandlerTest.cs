using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Campanhas.Ativar;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Ativar.Fakers;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Ativar.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Ativar;

public class AtivarCampanhaHandlerTest
{
    [Fact]
    public async Task Handle_WhenCampanhaIsCadastrada_ThenReturnOkWithStatusEmAndamento()
    {
        // Arrange
        var fixture = new AtivarCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmCadastrada();
        fixture.AppDbContextMock
            .SetupCampanhas([campanha])
            .SetupSaveChangesSuccess();

        // Act
        var result = await fixture.Handler.Handle(new AtivarCampanhaCommand(campanha.Id), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.StatusCode.ShouldBe(200);
        result.Dados.ShouldNotBeNull();
        result.Dados.Status.ShouldBe(StatusCampanha.EmAndamento);
        fixture.AppDbContextMock.VerifySaveChangesCalled();
    }

    [Fact]
    public async Task Handle_WhenCampanhaNotFound_ThenReturnNotFound()
    {
        // Arrange
        var fixture = new AtivarCampanhaHandlerFixture();
        fixture.AppDbContextMock.SetupCampanhas([]);

        // Act
        var result = await fixture.Handler.Handle(new AtivarCampanhaCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(404);
        result.Erro.ShouldBe(CampanhaErrorCodes.CampanhaNaoEncontrada);
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }

    [Fact]
    public async Task Handle_WhenDifferentGestor_ThenReturnNotFound()
    {
        // Arrange
        var fixture = new AtivarCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmCadastrada();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);
        fixture.CurrentUserMock.SetupUserId(Guid.NewGuid());

        // Act
        var result = await fixture.Handler.Handle(new AtivarCampanhaCommand(campanha.Id), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(404);
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }

    [Fact]
    public async Task Handle_WhenDomainException_ThenReturnFail()
    {
        // Arrange
        var fixture = new AtivarCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);

        // Act
        var result = await fixture.Handler.Handle(new AtivarCampanhaCommand(campanha.Id), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(400);
        result.Erro.ShouldBe(CampanhaErros.AtivacaoSomenteEmCadastrada);
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }
}
