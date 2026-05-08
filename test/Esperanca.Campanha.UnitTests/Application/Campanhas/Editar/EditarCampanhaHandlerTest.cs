using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Editar.Fakers;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Editar.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Editar;

public class EditarCampanhaHandlerTest
{
    [Fact]
    public async Task Handle_WhenValidCommand_ThenReturnOkWithUpdatedDto()
    {
        // Arrange
        var fixture = new EditarCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmCadastrada();
        fixture.AppDbContextMock
            .SetupCampanhas([campanha])
            .SetupSaveChangesSuccess();
        var command = EditarCampanhaCommandFaker.Valid(campanha.Id);

        // Act
        var result = await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.StatusCode.ShouldBe(200);
        result.Dados.ShouldNotBeNull();
        result.Dados.Titulo.ShouldBe(command.Titulo);
        result.Dados.MetaFinanceira.ShouldBe(command.MetaFinanceira);
        result.Dados.Status.ShouldBe(StatusCampanha.Cadastrada);
        fixture.AppDbContextMock.VerifySaveChangesCalled();
    }

    [Fact]
    public async Task Handle_WhenCampanhaNotFound_ThenReturnNotFound()
    {
        // Arrange
        var fixture = new EditarCampanhaHandlerFixture();
        fixture.AppDbContextMock.SetupCampanhas([]);
        var command = EditarCampanhaCommandFaker.Valid(Guid.NewGuid());

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
        var fixture = new EditarCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmCadastrada();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);
        fixture.CurrentUserMock.SetupUserId(Guid.NewGuid());
        var command = EditarCampanhaCommandFaker.Valid(campanha.Id);

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
        var fixture = new EditarCampanhaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);
        var command = EditarCampanhaCommandFaker.Valid(campanha.Id);

        // Act
        var result = await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(400);
        result.Erro.ShouldBe(CampanhaErros.EdicaoSomenteEmCadastrada);
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }
}
