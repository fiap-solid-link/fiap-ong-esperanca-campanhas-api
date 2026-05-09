using Esperanca.Campanha.UnitTests.WebApi.Transparencia._Shared.Fakers;
using Esperanca.Campanha.UnitTests.WebApi.Transparencia._Shared.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi.Transparencia.ListarCampanhas;

public class TransparenciaControllerListarCampanhasTest
{
    [Fact]
    public async Task ListarCampanhas_WhenHandlerReturnsSuccess_ThenReturn200ComLista()
    {
        // Arrange
        var fixture = new TransparenciaControllerFixture();
        var lista   = CampanhaTransparenciaDtoFaker.ListaValida();
        fixture.SenderMock.SetupListarCampanhasSuccess(lista);

        // Act
        var result = await fixture.Controller.ListarCampanhas(CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(200);
        objectResult.Value.ShouldBe(lista);
        fixture.SenderMock.VerifyListarCampanhasCalled();
    }
}
