using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fakers;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fixtures;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas.Prorrogar.Fakers;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas.Prorrogar;

public class CampanhaControllerProrrogarTest
{
    private static readonly Guid _id = Guid.Parse("11111111-0000-0000-0000-000000000001");

    [Fact]
    public async Task Prorrogar_WhenHandlerReturnsSuccess_ThenReturn200ComDto()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        var body    = ProrrogarCampanhaBodyFaker.Valid();
        var dto     = CampanhaDtoFaker.Valid();
        fixture.SenderMock.SetupProrrogarSuccess(dto);

        // Act
        var result = await fixture.Controller.Prorrogar(_id, body, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(200);
        objectResult.Value.ShouldBe(dto);
        fixture.SenderMock.VerifyProrrogarCalledWith(_id, ProrrogarCampanhaBodyFaker.NovaDataFim);
    }

    [Fact]
    public async Task Prorrogar_WhenHandlerReturnsFail_ThenReturn400()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        var body    = ProrrogarCampanhaBodyFaker.Valid();
        fixture.SenderMock.SetupProrrogarFail(CampanhaErros.ProrrogacaoSomenteEmAndamento);

        // Act
        var result = await fixture.Controller.Prorrogar(_id, body, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(400);
        objectResult.Value.ShouldBe(CampanhaErros.ProrrogacaoSomenteEmAndamento);
        fixture.SenderMock.VerifyProrrogarCalled();
    }

    [Fact]
    public async Task Prorrogar_WhenHandlerReturnsCampanhaNaoEncontrada_ThenReturn404()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        var body    = ProrrogarCampanhaBodyFaker.Valid();
        fixture.SenderMock.SetupProrrogarNotFound(CampanhaErrorCodes.CampanhaNaoEncontrada);

        // Act
        var result = await fixture.Controller.Prorrogar(_id, body, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(404);
        objectResult.Value.ShouldBe(CampanhaErrorCodes.CampanhaNaoEncontrada);
        fixture.SenderMock.VerifyProrrogarCalled();
    }
}
