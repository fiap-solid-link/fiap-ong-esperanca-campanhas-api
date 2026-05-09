using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Campanhas.Cancelar;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Cancelar.Fakers;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Cancelar.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Cancelar;

public class CancelarCampanhaHandlerTest
{
    [Fact]
    public async Task Handle_WhenCampanhaIsEmAndamento_ThenReturnOkWithStatusCancelada()
    {
        // Arrange
        var fixture = new CancelarCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento();
        fixture.AppDbContextMock
            .SetupCampanhas([campanha])
            .SetupSaveChangesSuccess();

        // Act
        var result = await fixture.Handler.Handle(new CancelarCampanhaCommand(campanha.Id), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.StatusCode.ShouldBe(200);
        result.Dados.ShouldNotBeNull();
        result.Dados.Status.ShouldBe(StatusCampanha.Cancelada);
        fixture.AppDbContextMock.VerifySaveChangesCalled();
    }

    [Fact]
    public async Task Handle_WhenCampanhaNotFound_ThenReturnNotFound()
    {
        // Arrange
        var fixture = new CancelarCampanhaHandlerFixture();
        fixture.AppDbContextMock.SetupCampanhas([]);

        // Act
        var result = await fixture.Handler.Handle(new CancelarCampanhaCommand(Guid.NewGuid()), CancellationToken.None);

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
        var fixture = new CancelarCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);
        fixture.CurrentUserMock.SetupUserId(Guid.NewGuid());

        // Act
        var result = await fixture.Handler.Handle(new CancelarCampanhaCommand(campanha.Id), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(404);
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }

    [Fact]
    public async Task Handle_WhenDomainException_ThenReturnFail()
    {
        // Arrange
        var fixture = new CancelarCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmCadastrada();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);

        // Act
        var result = await fixture.Handler.Handle(new CancelarCampanhaCommand(campanha.Id), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(400);
        result.Erro.ShouldBe(CampanhaErros.CancelamentoSomenteEmAndamento);
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }
}
