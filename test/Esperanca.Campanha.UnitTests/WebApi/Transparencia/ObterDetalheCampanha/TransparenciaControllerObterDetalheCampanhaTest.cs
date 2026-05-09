using Esperanca.Campanha.UnitTests.WebApi.Transparencia._Shared.Fakers;
using Esperanca.Campanha.UnitTests.WebApi.Transparencia._Shared.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi.Transparencia.ObterDetalheCampanha;

public class TransparenciaControllerObterDetalheCampanhaTest
{
    private static readonly Guid _campanhaId = Guid.Parse("cccccccc-0000-0000-0000-000000000001");

    [Fact]
    public async Task ObterDetalheCampanha_WhenHandlerReturnsSuccess_ThenReturn200ComDetalhe()
    {
        // Arrange
        var fixture = new TransparenciaControllerFixture();
        var detalhe = CampanhaDetalheDtoFaker.Valid();
        fixture.SenderMock.SetupObterDetalheSuccess(detalhe);

        // Act
        var result = await fixture.Controller.ObterDetalheCampanha(_campanhaId, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(200);
        objectResult.Value.ShouldBe(detalhe);
        fixture.SenderMock.VerifyObterDetalheCalled();
    }

    [Fact]
    public async Task ObterDetalheCampanha_WhenHandlerReturnsNotFound_ThenReturn404()
    {
        // Arrange
        var fixture = new TransparenciaControllerFixture();
        fixture.SenderMock.SetupObterDetalheNotFound("Campanha:404");

        // Act
        var result = await fixture.Controller.ObterDetalheCampanha(_campanhaId, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(404);
        fixture.SenderMock.VerifyObterDetalheCalled();
    }
}
