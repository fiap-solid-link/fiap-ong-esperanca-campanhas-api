using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.ProrrogarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Fiap.OngEsperanca.Campanhas.Api.Tests.Features;

public class ProrrogarCampanhaHandlerTests
{
    private readonly Mock<ICampanhaRepository> _repositoryMock;
    private readonly CampanhasDbContext _dbContextFake;
    private readonly ProrrogarCampanhaHandler _handler;

    public ProrrogarCampanhaHandlerTests()
    {
        _repositoryMock = new Mock<ICampanhaRepository>();

        var options = new DbContextOptionsBuilder<CampanhasDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContextFake = new CampanhasDbContext(options);

        _handler = new ProrrogarCampanhaHandler(_repositoryMock.Object, _dbContextFake);
    }

    [Fact(DisplayName = "Prorrogar: Deve prorrogar a campanha e retornar Sucesso quando ela estiver Em Andamento")]
    public async Task Handle_CampanhaEmAndamento_DeveProrrogarComSucesso()
    {
        // Arrange
        var novaDataFim = DateTime.UtcNow.AddDays(60);
        var comando = new ProrrogarCampanhaCommand(Guid.NewGuid(), novaDataFim);

        var campanhaFake = new Campanha("Teste", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), 1000m);
        // Precisamos ativar a campanha antes, porque só campanhas EmAndamento podem ser prorrogadas!
        campanhaFake.Ativar();

        _repositoryMock.Setup(r => r.ObterPorIdAsync(comando.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(campanhaFake);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.StatusCode.Should().Be(200);
        resultado.Sucesso.Should().BeTrue();

        // Verifica se a DataFim foi realmente atualizada
        campanhaFake.DataFim.Should().Be(novaDataFim);

        _repositoryMock.Verify(r => r.AtualizarAsync(campanhaFake, It.IsAny<CancellationToken>()), Times.Once);
    }
}