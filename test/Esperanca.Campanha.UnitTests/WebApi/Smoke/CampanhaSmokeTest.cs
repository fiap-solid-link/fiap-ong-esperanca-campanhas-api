using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Esperanca.Campanha.Application.Campanhas.Criar;
using Esperanca.Campanha.Domain.Campanhas;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi.Smoke;

public class CampanhaSmokeTest : IClassFixture<CampanhaWebApplicationFactory>
{
    private readonly CampanhaWebApplicationFactory _factory;

    public CampanhaSmokeTest(CampanhaWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Post_Campanha_ComRoleGestorONG_Retorna201()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = JwtTokenFactory.Create("GestorONG");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command = new CriarCampanhaCommand(
            "Campanha Smoke",
            "Descrição smoke",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(30),
            5000m,
            ModoEncerramento.PorDataOuMeta);

        // Act
        var response = await client.PostAsJsonAsync("/api/campanhas", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_Campanha_SemToken_Retorna401()
    {
        // Arrange
        var client = _factory.CreateClient();

        var command = new CriarCampanhaCommand(
            "Campanha Smoke",
            "Descrição smoke",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(30),
            5000m,
            ModoEncerramento.PorDataOuMeta);

        // Act
        var response = await client.PostAsJsonAsync("/api/campanhas", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_Campanha_ComRoleErrada_Retorna403()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = JwtTokenFactory.Create("Doador");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command = new CriarCampanhaCommand(
            "Campanha Smoke",
            "Descrição smoke",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(30),
            5000m,
            ModoEncerramento.PorDataOuMeta);

        // Act
        var response = await client.PostAsJsonAsync("/api/campanhas", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
