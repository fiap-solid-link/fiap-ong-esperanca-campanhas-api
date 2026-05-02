using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CancelarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Fiap.OngEsperanca.Campanhas.Api.Tests.Features;

public class CancelarCampanhaHandlerTests
{
    private readonly Mock<ICampanhaRepository> _repositoryMock;
    private readonly CampanhasDbContext _dbContextFake;
    private readonly CancelarCampanhaHandler _handler;

    public CancelarCampanhaHandlerTests()
    {
        _repositoryMock = new Mock<ICampanhaRepository>();

        var options = new DbContextOptionsBuilder<CampanhasDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContextFake = new CampanhasDbContext(options);

        _handler = new CancelarCampanhaHandler(_repositoryMock.Object, _dbContextFake);
    }

    [Fact(DisplayName = "Deve retornar falha 404 (Not Found) quando campanha nao for encontrada no banco")]
    public async Task Handle_CampanhaNaoExiste_DeveRetornar404()
    {
        // Arrange
        var comando = new CancelarCampanhaCommand(Guid.NewGuid());

        // Simulamos o banco de dados não encontrando a campanha
        _repositoryMock.Setup(r => r.ObterPorIdAsync(comando.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Campanha)null);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.StatusCode.Should().Be(404);
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Campanha não encontrada.");

        // Garantimos que o banco não foi atualizado indevidamente
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Campanha>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Deve cancelar a campanha, atualizar no repositório e retornar Sucesso")]
    public async Task Handle_CampanhaExiste_DeveCancelarComSucesso()
    {
        // Arrange
        var comando = new CancelarCampanhaCommand(Guid.NewGuid());
        var campanhaFake = new Campanha("Teste", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), 1000m);

        // Simulamos o banco de dados encontrando a campanha
        _repositoryMock.Setup(r => r.ObterPorIdAsync(comando.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(campanhaFake);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.StatusCode.Should().Be(200);
        resultado.Sucesso.Should().BeTrue();

        // Verifica se a regra de negócio do Domínio "Cancelar()" alterou o status corretamente
        campanhaFake.Status.Should().Be(StatusCampanha.Cancelada);

        // Garante que o Handler repassou a campanha atualizada para o repositório salvar
        _repositoryMock.Verify(r => r.AtualizarAsync(campanhaFake, It.IsAny<CancellationToken>()), Times.Once);
    }
}