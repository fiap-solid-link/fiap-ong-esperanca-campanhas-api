using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fakers;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas.Ativar;

public class CampanhaControllerAtivarTest
{
    private static readonly Guid _id = Guid.Parse("11111111-0000-0000-0000-000000000001");

    [Fact]
    public async Task Ativar_WhenHandlerReturnsSuccess_ThenReturn200ComDto()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        var dto     = CampanhaDtoFaker.Valid();
        fixture.SenderMock.SetupAtivarSuccess(dto);

        // Act
        var result = await fixture.Controller.Ativar(_id, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(200);
        objectResult.Value.ShouldBe(dto);
        fixture.SenderMock.VerifyAtivarCalled();
    }

    [Fact]
    public async Task Ativar_WhenHandlerReturnsFail_ThenReturn400()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        fixture.SenderMock.SetupAtivarFail(CampanhaErros.AtivacaoSomenteEmCadastrada);

        // Act
        var result = await fixture.Controller.Ativar(_id, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(400);
        objectResult.Value.ShouldBe(CampanhaErros.AtivacaoSomenteEmCadastrada);
        fixture.SenderMock.VerifyAtivarCalled();
    }

    [Fact]
    public async Task Ativar_WhenHandlerReturnsCampanhaNaoEncontrada_ThenReturn404()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        fixture.SenderMock.SetupAtivarNotFound(CampanhaErrorCodes.CampanhaNaoEncontrada);

        // Act
        var result = await fixture.Controller.Ativar(_id, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(404);
        objectResult.Value.ShouldBe(CampanhaErrorCodes.CampanhaNaoEncontrada);
        fixture.SenderMock.VerifyAtivarCalled();
    }
}
