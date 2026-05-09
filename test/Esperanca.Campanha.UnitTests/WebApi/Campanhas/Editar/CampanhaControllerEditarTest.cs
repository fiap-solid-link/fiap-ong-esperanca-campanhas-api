using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fakers;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fixtures;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas.Editar.Fakers;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas.Editar;

public class CampanhaControllerEditarTest
{
    private static readonly Guid _id = Guid.Parse("11111111-0000-0000-0000-000000000001");

    [Fact]
    public async Task Editar_WhenHandlerReturnsSuccess_ThenReturn200ComDto()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        var body    = EditarCampanhaBodyFaker.Valid();
        var dto     = CampanhaDtoFaker.Valid();
        fixture.SenderMock.SetupEditarSuccess(dto);

        // Act
        var result = await fixture.Controller.Editar(_id, body, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(200);
        objectResult.Value.ShouldBe(dto);
        fixture.SenderMock.VerifyEditarCalledWith(_id);
    }

    [Fact]
    public async Task Editar_WhenHandlerReturnsFail_ThenReturn400()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        var body    = EditarCampanhaBodyFaker.Valid();
        fixture.SenderMock.SetupEditarFail(CampanhaErros.EdicaoSomenteEmCadastrada);

        // Act
        var result = await fixture.Controller.Editar(_id, body, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(400);
        objectResult.Value.ShouldBe(CampanhaErros.EdicaoSomenteEmCadastrada);
        fixture.SenderMock.VerifyEditarCalled();
    }

    [Fact]
    public async Task Editar_WhenHandlerReturnsCampanhaNaoEncontrada_ThenReturn404()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        var body    = EditarCampanhaBodyFaker.Valid();
        fixture.SenderMock.SetupEditarNotFound(CampanhaErrorCodes.CampanhaNaoEncontrada);

        // Act
        var result = await fixture.Controller.Editar(_id, body, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(404);
        objectResult.Value.ShouldBe(CampanhaErrorCodes.CampanhaNaoEncontrada);
        fixture.SenderMock.VerifyEditarCalled();
    }
}
