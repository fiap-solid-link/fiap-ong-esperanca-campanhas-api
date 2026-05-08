using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using Esperanca.Campanha.UnitTests.Application.Doacoes.EnviarIntencao.Fakers;
using Esperanca.Campanha.UnitTests.Application.Doacoes.EnviarIntencao.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes.EnviarIntencao;

public class EnviarIntencaoDoacaoHandlerTest
{
    [Fact]
    public async Task Handle_WhenCampanhaEmAndamento_ThenPublicaEventoERetorna202()
    {
        // Arrange
        var fixture = new EnviarIntencaoDoacaoHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);
        var command = EnviarIntencaoDoacaoCommandFaker.Valid(campanha.Id);

        // Act
        var result = await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.StatusCode.ShouldBe(202);
        result.Dados.ShouldNotBeNull();
        result.Dados.IdDoacao.ShouldNotBe(Guid.Empty);
        result.Dados.IdempotencyKey.ShouldNotBe(Guid.Empty);
        result.Dados.DataIntencao.ShouldBe(DateTimeProviderMock.DefaultNow);

        fixture.DoacaoPublisherMock.VerifyPublicarRecebidaCalledWith(e =>
            e.IdCampanha == campanha.Id
            && e.IdDoador == CurrentUserMock.DefaultUserId
            && e.Valor == command.Valor
            && e.IdDoacao == result.Dados.IdDoacao
            && e.IdempotencyKey == result.Dados.IdempotencyKey
            && e.DataIntencao == DateTimeProviderMock.DefaultNow);
    }

    [Fact]
    public async Task Handle_WhenCampanhaNaoExiste_ThenRetornaNotFoundENaoPublica()
    {
        // Arrange
        var fixture = new EnviarIntencaoDoacaoHandlerFixture();
        fixture.AppDbContextMock.SetupCampanhas([]);
        var command = EnviarIntencaoDoacaoCommandFaker.Valid(Guid.NewGuid());

        // Act
        var result = await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(404);
        result.Erro.ShouldBe(CampanhaErrorCodes.CampanhaNaoEncontrada);
        fixture.DoacaoPublisherMock.VerifyPublicarRecebidaNotCalled();
    }

    [Fact]
    public async Task Handle_WhenCampanhaCadastrada_ThenRetornaFailENaoPublica()
    {
        // Arrange
        var fixture = new EnviarIntencaoDoacaoHandlerFixture();
        var campanha = CampanhaFaker.EmCadastrada();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);
        var command = EnviarIntencaoDoacaoCommandFaker.Valid(campanha.Id);

        // Act
        var result = await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeFalse();
        result.StatusCode.ShouldBe(400);
        result.Erro.ShouldBe(CampanhaErros.ArrecadacaoSomenteEmAndamento);
        fixture.DoacaoPublisherMock.VerifyPublicarRecebidaNotCalled();
    }

    [Fact]
    public async Task Handle_WhenPublisherThrows_ThenPropagaExcecao()
    {
        // Arrange
        var fixture = new EnviarIntencaoDoacaoHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento();
        fixture.AppDbContextMock.SetupCampanhas([campanha]);
        fixture.DoacaoPublisherMock.SetupPublicarRecebidaThrows(new InvalidOperationException("broker offline"));
        var command = EnviarIntencaoDoacaoCommandFaker.Valid(campanha.Id);

        // Act
        var act = () => fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        await Should.ThrowAsync<InvalidOperationException>(act);
        fixture.DoacaoPublisherMock.VerifyPublicarRecebidaCalled();
    }
}
