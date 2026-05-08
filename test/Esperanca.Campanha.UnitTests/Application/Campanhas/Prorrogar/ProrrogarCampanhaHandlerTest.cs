using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Campanhas.Prorrogar;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Prorrogar.Fakers;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Prorrogar.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Prorrogar;

public class ProrrogarCampanhaHandlerTest
{
    [Fact]
    public async Task Handle_WhenValidCommand_ThenReturnOkWithNewDataFim()
    {
        // Arrange
        var fixture = new ProrrogarCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento();
        fixture.AppDbContextMock
            .SetupCampanhas([campanha])
            .SetupSaveChangesSuccess();
        var command = ProrrogarCampanhaCommandFaker.Valid(campanha.Id);

        // Act
        var result = await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.StatusCode.ShouldBe(200);
        result.Dados.ShouldNotBeNull();
        result.Dados.DataFim.ShouldBe(command.NovaDataFim);
        fixture.AppDbContextMock.VerifySaveChangesCalled();
    }

    [Fact]
    public async Task Handle_WhenCampanhaNotFound_ThenReturnNotFound()
    {
        // Arrange
        var fixture = new ProrrogarCampanhaHandlerFixture();
        fixture.AppDbContextMock.SetupCampanhas([]);
        var command = ProrrogarCampanhaCommandFaker.Valid(Guid.NewGuid());

        // Act
        var result = await fixture.Handler.Handle(command, CancellationToken.None);

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
        var fixture = new ProrrogarCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);
        fixture.CurrentUserMock.SetupUserId(Guid.NewGuid());
        var command = ProrrogarCampanhaCommandFaker.Valid(campanha.Id);

        // Act
        var result = await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(404);
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }

    [Fact]
    public async Task Handle_WhenDomainException_ThenReturnFail()
    {
        // Arrange
        var fixture = new ProrrogarCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmCadastrada();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);
        var command = ProrrogarCampanhaCommandFaker.Valid(campanha.Id);

        // Act
        var result = await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(400);
        result.Erro.ShouldBe(CampanhaErros.ProrrogacaoSomenteEmAndamento);
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }
}
