using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fakers;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas.Obter;

public class CampanhaControllerObterTest
{
    private static readonly Guid _id = Guid.Parse("11111111-0000-0000-0000-000000000001");

    [Fact]
    public async Task Obter_WhenHandlerReturnsSuccess_ThenReturn200ComDto()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        var dto     = CampanhaDtoFaker.Valid();
        fixture.SenderMock.SetupObterSuccess(dto);

        // Act
        var result = await fixture.Controller.Obter(_id, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(200);
        objectResult.Value.ShouldBe(dto);
        fixture.SenderMock.VerifyObterCalled();
    }

    [Fact]
    public async Task Obter_WhenHandlerReturnsCampanhaNaoEncontrada_ThenReturn404()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        fixture.SenderMock.SetupObterNotFound(CampanhaErrorCodes.CampanhaNaoEncontrada);

        // Act
        var result = await fixture.Controller.Obter(_id, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(404);
        objectResult.Value.ShouldBe(CampanhaErrorCodes.CampanhaNaoEncontrada);
        fixture.SenderMock.VerifyObterCalled();
    }
}
