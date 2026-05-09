using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fakers;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas.Cancelar;

public class CampanhaControllerCancelarTest
{
    private static readonly Guid _id = Guid.Parse("11111111-0000-0000-0000-000000000001");

    [Fact]
    public async Task Cancelar_WhenHandlerReturnsSuccess_ThenReturn200ComDto()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        var dto     = CampanhaDtoFaker.Valid();
        fixture.SenderMock.SetupCancelarSuccess(dto);

        // Act
        var result = await fixture.Controller.Cancelar(_id, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(200);
        objectResult.Value.ShouldBe(dto);
        fixture.SenderMock.VerifyCancelarCalled();
    }

    [Fact]
    public async Task Cancelar_WhenHandlerReturnsFail_ThenReturn400()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        fixture.SenderMock.SetupCancelarFail(CampanhaErros.CancelamentoSomenteEmAndamento);

        // Act
        var result = await fixture.Controller.Cancelar(_id, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(400);
        objectResult.Value.ShouldBe(CampanhaErros.CancelamentoSomenteEmAndamento);
        fixture.SenderMock.VerifyCancelarCalled();
    }

    [Fact]
    public async Task Cancelar_WhenHandlerReturnsCampanhaNaoEncontrada_ThenReturn404()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        fixture.SenderMock.SetupCancelarNotFound(CampanhaErrorCodes.CampanhaNaoEncontrada);

        // Act
        var result = await fixture.Controller.Cancelar(_id, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(404);
        objectResult.Value.ShouldBe(CampanhaErrorCodes.CampanhaNaoEncontrada);
        fixture.SenderMock.VerifyCancelarCalled();
    }
}
