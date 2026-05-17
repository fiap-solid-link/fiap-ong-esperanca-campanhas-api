using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Criar.Fakers;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Criar.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Criar;

public class CriarCampanhaHandlerTest
{
    [Fact]
    public async Task Handle_WhenValidCommand_ThenReturnCreatedWithDto()
    {
        // Arrange
        var fixture = new CriarCampanhaHandlerFixture();
        var command = CriarCampanhaCommandFaker.Valid();
        fixture.AppDbContextMock
            .SetupCampanhas([])
            .SetupSaveChangesSuccess();

        // Act
        var result = await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.StatusCode.ShouldBe(201);
        result.Dados.ShouldNotBeNull();
        result.Dados.Titulo.ShouldBe(command.Titulo);
        result.Dados.Descricao.ShouldBe(command.Descricao);
        result.Dados.MetaFinanceira.ShouldBe(command.MetaFinanceira);
        result.Dados.Status.ShouldBe(StatusCampanha.Cadastrada);
        result.Dados.IdGestor.ShouldBe(fixture.CurrentUserMock.Instance.UserId);
        fixture.AppDbContextMock.VerifyCampanhaAdded();
        fixture.AppDbContextMock.VerifySaveChangesCalled();
        fixture.TransparenciaProjectionWriterMock.VerifyCriarProjecaoCampanhaCalled();
    }

    [Fact]
    public async Task Handle_WhenDomainException_ThenReturnFail()
    {
        // Arrange
        var fixture = new CriarCampanhaHandlerFixture();
        var command = CriarCampanhaCommandFaker.ComDataFimNoPassado();
        fixture.AppDbContextMock.SetupSaveChangesSuccess();

        // Act
        var result = await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(400);
        result.Erro.ShouldBe(CampanhaErros.DataFimMenorQueAgora);
        fixture.AppDbContextMock.VerifyCampanhaNotAdded();
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
        fixture.TransparenciaProjectionWriterMock.VerifyCriarProjecaoCampanhaNotCalled();
    }
}
