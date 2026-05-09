using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.WebApi.Doacoes.EnviarIntencao.Fakers;
using Esperanca.Campanha.UnitTests.WebApi.Doacoes.EnviarIntencao.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi.Doacoes.EnviarIntencao;

public class DoacaoControllerTest
{
    [Fact]
    public async Task EnviarIntencao_WhenHandlerReturnsAccepted_ThenReturn202ComDto()
    {
        // Arrange
        var fixture = new DoacaoControllerFixture();
        var command = EnviarIntencaoDoacaoCommandFaker.Valid();
        var dto     = IntencaoDoacaoDtoFaker.Valid();
        fixture.SenderMock.SetupEnviarIntencaoSuccess(dto);

        // Act
        var result = await fixture.Controller.EnviarIntencao(command, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(202);
        objectResult.Value.ShouldBe(dto);
        fixture.SenderMock.VerifyEnviarIntencaoCalled();
    }

    [Fact]
    public async Task EnviarIntencao_WhenHandlerReturnsCampanhaNaoEncontrada_ThenReturn404()
    {
        // Arrange
        var fixture = new DoacaoControllerFixture();
        var command = EnviarIntencaoDoacaoCommandFaker.Valid();
        fixture.SenderMock.SetupEnviarIntencaoNotFound(CampanhaErrorCodes.CampanhaNaoEncontrada);

        // Act
        var result = await fixture.Controller.EnviarIntencao(command, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(404);
        objectResult.Value.ShouldBe(CampanhaErrorCodes.CampanhaNaoEncontrada);
        fixture.SenderMock.VerifyEnviarIntencaoCalled();
    }

    [Fact]
    public async Task EnviarIntencao_WhenHandlerReturnsCampanhaNotEmAndamento_ThenReturn400()
    {
        // Arrange
        var fixture = new DoacaoControllerFixture();
        var command = EnviarIntencaoDoacaoCommandFaker.Valid();
        fixture.SenderMock.SetupEnviarIntencaoFail(CampanhaErros.ArrecadacaoSomenteEmAndamento);

        // Act
        var result = await fixture.Controller.EnviarIntencao(command, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(400);
        objectResult.Value.ShouldBe(CampanhaErros.ArrecadacaoSomenteEmAndamento);
        fixture.SenderMock.VerifyEnviarIntencaoCalled();
    }
}
