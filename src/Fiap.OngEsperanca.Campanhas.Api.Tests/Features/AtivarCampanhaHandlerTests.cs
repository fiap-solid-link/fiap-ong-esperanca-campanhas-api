using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.AtivarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Fiap.OngEsperanca.Campanhas.Api.Tests.Features;

public class AtivarCampanhaHandlerTests
{
    private readonly Mock<ICampanhaRepository> _repositoryMock;
    private readonly CampanhasDbContext _dbContextFake;
    private readonly AtivarCampanhaHandler _handler;

    public AtivarCampanhaHandlerTests()
    {
        _repositoryMock = new Mock<ICampanhaRepository>();

        var options = new DbContextOptionsBuilder<CampanhasDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContextFake = new CampanhasDbContext(options);

        _handler = new AtivarCampanhaHandler(_repositoryMock.Object, _dbContextFake);
    }

    [Fact(DisplayName = "Ativar: Deve ativar a campanha e retornar Sucesso")]
    public async Task Handle_CampanhaCadastrada_DeveAtivarComSucesso()
    {
        // Arrange
        var comando = new AtivarCampanhaCommand(Guid.NewGuid());
        var campanhaFake = new Campanha("Teste", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), 1000m);

        _repositoryMock.Setup(r => r.ObterPorIdAsync(comando.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(campanhaFake);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.StatusCode.Should().Be(200);
        resultado.Sucesso.Should().BeTrue();

        // Verifica se a regra de domínio foi aplicada (mudou para EmAndamento)
        campanhaFake.Status.Should().Be(StatusCampanha.EmAndamento);

        _repositoryMock.Verify(r => r.AtualizarAsync(campanhaFake, It.IsAny<CancellationToken>()), Times.Once);
    }
}