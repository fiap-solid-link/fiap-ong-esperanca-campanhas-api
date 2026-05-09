using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fixtures;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas.Listar.Fakers;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas.Listar;

public class CampanhaControllerListarTest
{
    [Fact]
    public async Task Listar_WhenHandlerReturnsSuccess_ThenReturn200ComPagina()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        var pagina  = PaginaCampanhasDtoFaker.Valid();
        fixture.SenderMock.SetupListarSuccess(pagina);

        // Act
        var result = await fixture.Controller.Listar(ct: CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(200);
        objectResult.Value.ShouldBe(pagina);
        fixture.SenderMock.VerifyListarCalled();
    }

    [Fact]
    public async Task Listar_WhenHandlerReturnsFail_ThenReturn400()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        fixture.SenderMock.SetupListarFail(CampanhaErrorCodes.ErroValidacao);

        // Act
        var result = await fixture.Controller.Listar(ct: CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(400);
        objectResult.Value.ShouldBe(CampanhaErrorCodes.ErroValidacao);
        fixture.SenderMock.VerifyListarCalled();
    }
}
