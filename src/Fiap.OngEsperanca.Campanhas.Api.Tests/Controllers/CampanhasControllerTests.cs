using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using Fiap.OngEsperanca.Campanhas.Api.Features.Doacoes.EnviarIntencao;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CriarCampanha;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Fiap.OngEsperanca.Campanhas.Api.Tests.Controllers;

public class CampanhasControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CampanhasController _controller;

    public CampanhasControllerTests()
    {
        // 1. Arrange: Criamos um "Dublê" do MediatR
        _mediatorMock = new Mock<IMediator>();

        // 2. Injetamos o MediatR falso dentro do Controller real
        _controller = new CampanhasController(_mediatorMock.Object);
    }

    [Fact(DisplayName = "Deve retornar HTTP 202 (Accepted) quando a doação for enviada com sucesso")]
    public async Task Doar_QuandoSucesso_DeveRetornarAccepted()
    {
        // Arrange
        var id = Guid.NewGuid();
        // Simulamos o payload que o usuário enviaria (sem a campanhaId, pois ela vem da URL)
        var comando = new EnviarIntencaoDoacaoCommand(Guid.Empty, Guid.NewGuid(), 150m);

        // Ensinamos o nosso MediatR fake a retornar um Result de sucesso igualzinho a sua API real faz
        var resultadoSucesso = Result<string>.Ok("Intenção de doação enviada para processamento com sucesso.");
        _mediatorMock.Setup(m => m.Send(It.IsAny<EnviarIntencaoDoacaoCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(resultadoSucesso);

        // Act
        // Chamamos o método correto "Doar" passando o ID pela URL e o JSON no comando
        var resultado = await _controller.Doar(id, comando, CancellationToken.None);

        // Assert
        // Verificamos se a API respondeu com um ObjectResult e se o Status Code foi exatamente o 202
        var objectResult = resultado.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(202);

        // Garantimos que o Controller repassou a bola para o MediatR exatamente 1 vez
        _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarIntencaoDoacaoCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}