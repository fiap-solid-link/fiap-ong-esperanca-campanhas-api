using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Events;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Services;
using Fiap.OngEsperanca.Campanhas.Api.Features.Doacoes.EnviarIntencao;
using FluentAssertions;
using Moq;
using Xunit;

namespace Fiap.OngEsperanca.Campanhas.Api.Tests.Features;

public class EnviarIntencaoDoacaoHandlerTests
{
    private readonly Mock<ICampanhaRepository> _campanhaRepositoryMock;
    private readonly Mock<IMessageBusService> _messageBusServiceMock;
    private readonly EnviarIntencaoDoacaoHandler _handler;

    public EnviarIntencaoDoacaoHandlerTests()
    {
        _campanhaRepositoryMock = new Mock<ICampanhaRepository>();
        _messageBusServiceMock = new Mock<IMessageBusService>();

        _handler = new EnviarIntencaoDoacaoHandler(_campanhaRepositoryMock.Object, _messageBusServiceMock.Object);
    }

    [Fact(DisplayName = "Deve publicar evento no RabbitMQ quando a campanha existir e estiver ativa")]
    public async Task Handle_CampanhaValida_DevePublicarEvento()
    {
        // Arrange
        var comando = new EnviarIntencaoDoacaoCommand(Guid.NewGuid(), Guid.NewGuid(), 50m);

        // CORREÇÃO: Adicionando as datas que o construtor exige
        var campanhaFake = new Campanha("Campanha Teste", "Descricao", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), 1000m);

        _campanhaRepositoryMock.Setup(repo => repo.ObterPorIdAsync(comando.CampanhaId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(campanhaFake);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();

        _messageBusServiceMock.Verify(bus => bus.PublicarAsync(
            It.IsAny<DoacaoRecebidaEvent>(),
            "doacoes-recebidas"),
            Times.Once);
    }

    [Fact(DisplayName = "NÃO deve publicar no RabbitMQ se a campanha não for encontrada")]
    public async Task Handle_CampanhaNaoExiste_NaoDevePublicarEvento()
    {
        // Arrange
        var comando = new EnviarIntencaoDoacaoCommand(Guid.NewGuid(), Guid.NewGuid(), 50m);

        // Simulamos o banco de dados retornando NULL (Campanha não encontrada)
        _campanhaRepositoryMock.Setup(repo => repo.ObterPorIdAsync(comando.CampanhaId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync((Campanha)null);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();

        // GARANTIA MÁXIMA: O RabbitMQ NUNCA deve ser chamado (Times.Never)
        _messageBusServiceMock.Verify(bus => bus.PublicarAsync(
            It.IsAny<DoacaoRecebidaEvent>(),
            It.IsAny<string>()),
            Times.Never);
    }

}