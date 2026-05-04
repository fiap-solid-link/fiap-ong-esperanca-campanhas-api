using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.EditarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Fiap.OngEsperanca.Campanhas.Api.Tests.Features;

public class EditarCampanhaHandlerTests
{
    private readonly Mock<ICampanhaRepository> _repositoryMock;
    private readonly CampanhasDbContext _dbContextFake;
    private readonly EditarCampanhaHandler _handler;

    public EditarCampanhaHandlerTests()
    {
        _repositoryMock = new Mock<ICampanhaRepository>();

        var options = new DbContextOptionsBuilder<CampanhasDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContextFake = new CampanhasDbContext(options);

        _handler = new EditarCampanhaHandler(_repositoryMock.Object, _dbContextFake);
    }

    [Fact(DisplayName = "Editar: Deve retornar falha 404 quando campanha não for encontrada")]
    public async Task Handle_CampanhaNaoExiste_DeveRetornar404()
    {
        // Arrange
        var comando = new EditarCampanhaCommand(Guid.NewGuid(), "Novo", "Desc", 2000m);
        _repositoryMock.Setup(r => r.ObterPorIdAsync(comando.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Campanha)null);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.StatusCode.Should().Be(404);
        resultado.Sucesso.Should().BeFalse();
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Campanha>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Editar: Deve editar a campanha e retornar Sucesso quando ela estiver Cadastrada")]
    public async Task Handle_CampanhaCadastrada_DeveEditarComSucesso()
    {
        // Arrange
        var comando = new EditarCampanhaCommand(Guid.NewGuid(), "Novo Título", "Nova Descrição", 5000m);

        // A campanha já nasce como Cadastrada, o que atende a regra de edição
        var campanhaFake = new Campanha("Velho", "Velho", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), 1000m);

        _repositoryMock.Setup(r => r.ObterPorIdAsync(comando.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(campanhaFake);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.StatusCode.Should().Be(200);
        resultado.Sucesso.Should().BeTrue();

        // Verifica se a entidade realmente mudou os valores
        campanhaFake.Titulo.Should().Be("Novo Título");
        campanhaFake.MetaFinanceira.Should().Be(5000m);

        _repositoryMock.Verify(r => r.AtualizarAsync(campanhaFake, It.IsAny<CancellationToken>()), Times.Once);
    }
}