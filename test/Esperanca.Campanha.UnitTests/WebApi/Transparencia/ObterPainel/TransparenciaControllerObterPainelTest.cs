using Esperanca.Campanha.UnitTests.WebApi.Transparencia._Shared.Fakers;
using Esperanca.Campanha.UnitTests.WebApi.Transparencia._Shared.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi.Transparencia.ObterPainel;

public class TransparenciaControllerObterPainelTest
{
    [Fact]
    public async Task ObterPainel_WhenHandlerReturnsSuccess_ThenReturn200ComPainel()
    {
        // Arrange
        var fixture = new TransparenciaControllerFixture();
        var painel  = PainelMacroDtoFaker.Valid();
        fixture.SenderMock.SetupObterPainelSuccess(painel);

        // Act
        var result = await fixture.Controller.ObterPainel(CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(200);
        objectResult.Value.ShouldBe(painel);
        fixture.SenderMock.VerifyObterPainelCalled();
    }
}
