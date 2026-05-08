using Esperanca.Campanha.Application.Transparencia.ConsultarPainelMacro;
using Esperanca.Campanha.UnitTests.Application.Transparencia._Shared.Fakers;
using Esperanca.Campanha.UnitTests.Application.Transparencia.ConsultarPainelMacro.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Transparencia.ConsultarPainelMacro;

public class ConsultarPainelMacroHandlerTest
{
    [Fact]
    public async Task Handle_WhenPainelExiste_ThenRetornaDadosDoMongo()
    {
        // Arrange
        var fixture = new ConsultarPainelMacroHandlerFixture();
        var painel = TransparenciaFaker.PainelMacroComDados();
        fixture.RepositoryMock.SetupPainelMacro(painel);

        // Act
        var result = await fixture.Handler.Handle(new ConsultarPainelMacroQuery(), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.StatusCode.ShouldBe(200);
        result.Dados.ShouldBe(painel);
    }

    [Fact]
    public async Task Handle_WhenMongoVazio_ThenRetornaPainelZerado()
    {
        // Arrange
        var fixture = new ConsultarPainelMacroHandlerFixture();
        fixture.RepositoryMock.SetupPainelMacro(null);

        // Act
        var result = await fixture.Handler.Handle(new ConsultarPainelMacroQuery(), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.StatusCode.ShouldBe(200);
        result.Dados.ShouldNotBeNull();
        result.Dados.TotalArrecadado.ShouldBe(0m);
        result.Dados.TotalDoacoes.ShouldBe(0);
        result.Dados.TotalCampanhasAtivas.ShouldBe(0);
        result.Dados.TotalCampanhasConcluidas.ShouldBe(0);
        result.Dados.TopDoadores.ShouldBeEmpty();
    }
}
