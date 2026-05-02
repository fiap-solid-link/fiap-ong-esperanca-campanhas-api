using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CriarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Fiap.OngEsperanca.Campanhas.Api.Tests.Features;

public class CriarCampanhaHandlerTests
{
    private readonly Mock<ICampanhaRepository> _repositoryMock;
    private readonly CampanhasDbContext _dbContextFake;
    private readonly CriarCampanhaHandler _handler;

    public CriarCampanhaHandlerTests()
    {
        // Mockamos o repositório como já fizemos antes
        _repositoryMock = new Mock<ICampanhaRepository>();

        // Criamos um banco de dados falso (In-Memory) zerado para cada teste
        var options = new DbContextOptionsBuilder<CampanhasDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContextFake = new CampanhasDbContext(options);

        // Injetamos as peças no Handler
        _handler = new CriarCampanhaHandler(_repositoryMock.Object, _dbContextFake);
    }

    [Fact(DisplayName = "Deve criar a campanha, salvar no repositório e retornar Result.Created")]
    public async Task Handle_QuandoDadosValidos_DeveSalvarERetornar201()
    {
        // Arrange
        var comando = new CriarCampanhaCommand("Agasalho 2026", "Doações de inverno", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), 5000m);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();

        // Verifica se o Handler gerou o status HTTP 201 e marcou como Sucesso
        resultado.StatusCode.Should().Be(201);
        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Titulo.Should().Be("Agasalho 2026");

        // Garante que o método "AdicionarAsync" do Repositório foi chamado exatamente 1 vez
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Campanha>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}