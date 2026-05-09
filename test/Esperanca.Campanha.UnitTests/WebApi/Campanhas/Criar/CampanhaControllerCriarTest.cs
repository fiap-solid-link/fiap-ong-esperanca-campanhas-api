using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fakers;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fixtures;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas.Criar.Fakers;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas.Criar;

public class CampanhaControllerCriarTest
{
    [Fact]
    public async Task Criar_WhenHandlerReturnsCreated_ThenReturn201ComDto()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        var command = CriarCampanhaCommandFaker.Valid();
        var dto     = CampanhaDtoFaker.Valid();
        fixture.SenderMock.SetupCriarSuccess(dto);

        // Act
        var result = await fixture.Controller.Criar(command, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(201);
        objectResult.Value.ShouldBe(dto);
        fixture.SenderMock.VerifyCriarCalled();
    }

    [Fact]
    public async Task Criar_WhenHandlerReturnsFail_ThenReturn400()
    {
        // Arrange
        var fixture = new CampanhaControllerFixture();
        var command = CriarCampanhaCommandFaker.Valid();
        fixture.SenderMock.SetupCriarFail(CampanhaErros.TituloObrigatorio);

        // Act
        var result = await fixture.Controller.Criar(command, CancellationToken.None);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(400);
        objectResult.Value.ShouldBe(CampanhaErros.TituloObrigatorio);
        fixture.SenderMock.VerifyCriarCalled();
    }
}
