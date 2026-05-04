using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Features.Doacoes.EnviarIntencao;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CancelarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CriarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.ListarCampanhas;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.AtivarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.EditarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.ProrrogarCampanha;
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

    // --- TESTE 2: CRIAR CAMPANHA ---
    [Fact(DisplayName = "Deve retornar HTTP 201 (Created) ao criar campanha com sucesso")]
    public async Task Criar_QuandoSucesso_DeveRetornarCreated()
    {
        // Arrange
        var comando = new CriarCampanhaCommand(
            "Campanha de Teste",
            "Descrição Teste",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30),
            5000m);

        var responseEsperado = new CriarCampanhaResponse(Guid.NewGuid(), "Campanha de Teste");

        // Simulamos o MediatR retornando o Result.Created (Status 201) que o seu Handler produz
        var resultadoSucesso = Result<CriarCampanhaResponse>.Created(responseEsperado);

        _mediatorMock.Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(resultadoSucesso);

        // Act
        var resultado = await _controller.Criar(comando, CancellationToken.None);

        // Assert
        var objectResult = resultado.Should().BeOfType<ObjectResult>().Subject;

        // Verifica se o Controller pegou o Status 201 do Result e aplicou no HTTP
        objectResult.StatusCode.Should().Be(201);

        // Verifica se o Controller devolveu o DTO "CriarCampanhaResponse" corretamente
        objectResult.Value.Should().BeEquivalentTo(responseEsperado);
    }

    // --- TESTE 3: LISTAR CAMPANHAS ---
    [Fact(DisplayName = "Deve retornar HTTP 200 (OK) com a lista de campanhas")]
    public async Task Listar_QuandoSucesso_DeveRetornarOkComLista()
    {
        // Arrange
        var query = new ListarCampanhasQuery();

        // Criamos uma lista fake com 1 campanha para o teste
        var listaFake = new List<CampanhaResponse>
        {
            new CampanhaResponse(Guid.NewGuid(), "Campanha 1", "Desc", 1000m, 150m, StatusCampanha.EmAndamento)
        };

        // O MediatR vai devolver o Result de sucesso contendo a nossa lista
        var resultadoSucesso = Result<IEnumerable<CampanhaResponse>>.Ok(listaFake);

        _mediatorMock.Setup(m => m.Send(It.IsAny<ListarCampanhasQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(resultadoSucesso);

        // Act
        var resultado = await _controller.Listar(CancellationToken.None);

        // Assert
        var objectResult = resultado.Should().BeOfType<ObjectResult>().Subject;

        // Status 200 (OK) é o padrão do método Result.Ok()
        objectResult.StatusCode.Should().Be(200);

        // Garantimos que a lista devolvida no Value do ObjectResult é a mesma que mockamos
        objectResult.Value.Should().BeEquivalentTo(listaFake);
    }

    // --- TESTE 4: CANCELAR CAMPANHA ---
    [Fact(DisplayName = "Deve retornar HTTP 200 (OK) ao cancelar campanha com sucesso")]
    public async Task Cancelar_QuandoSucesso_DeveRetornarOk()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Simulamos a resposta de sucesso do Handler
        var resultadoSucesso = Result<string>.Ok("Campanha cancelada com sucesso.");

        // Ensinamos o MediatR a retornar sucesso quando receber um comando com esse ID específico
        _mediatorMock.Setup(m => m.Send(It.Is<CancelarCampanhaCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(resultadoSucesso);

        // Act
        var resultado = await _controller.Cancelar(id, CancellationToken.None);

        // Assert
        var objectResult = resultado.Should().BeOfType<ObjectResult>().Subject;

        // O método Result.Ok() devolve 200 por padrão, e o Controller espelha isso
        objectResult.StatusCode.Should().Be(200);

        // Verificamos se o Controller realmente enviou o comando para o MediatR com o ID correto
        _mediatorMock.Verify(m => m.Send(It.Is<CancelarCampanhaCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
    }

    // ====================================================================
    // --- TESTES DE CAMINHO TRISTE (SAD PATHS) ---
    // ====================================================================

    [Fact(DisplayName = "Doar: Deve retornar HTTP 404 (Not Found) quando campanha não existir")]
    public async Task Doar_QuandoCampanhaNaoExiste_DeveRetornarNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var comando = new EnviarIntencaoDoacaoCommand(Guid.Empty, Guid.NewGuid(), 150m);

        // Simulamos o Handler dizendo "Deu ruim, status 404"
        var resultadoFalha = Result<string>.Fail("Campanha não encontrada.", 404);

        _mediatorMock.Setup(m => m.Send(It.IsAny<EnviarIntencaoDoacaoCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(resultadoFalha);

        // Act
        var resultado = await _controller.Doar(id, comando, CancellationToken.None);

        // Assert
        var objectResult = resultado.Should().BeOfType<ObjectResult>().Subject;

        // Verifica se repassou o 404 corretamente
        objectResult.StatusCode.Should().Be(404);

        // Verifica se a estrutura do erro está no formato { erro = "mensagem" } que você definiu
        objectResult.Value.Should().BeEquivalentTo(new { erro = "Campanha não encontrada." });
    }

    [Fact(DisplayName = "Cancelar: Deve retornar HTTP 400 (Bad Request) quando houver regra de negócio violada")]
    public async Task Cancelar_QuandoFalhaDeNegocio_DeveRetornarBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Simulamos o Handler devolvendo um status 400 (ex: campanha já estava cancelada)
        var resultadoFalha = Result<string>.Fail("Não é possível cancelar uma campanha já encerrada.", 400);

        _mediatorMock.Setup(m => m.Send(It.Is<CancelarCampanhaCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(resultadoFalha);

        // Act
        var resultado = await _controller.Cancelar(id, CancellationToken.None);

        // Assert
        var objectResult = resultado.Should().BeOfType<ObjectResult>().Subject;

        objectResult.StatusCode.Should().Be(400);
        objectResult.Value.Should().BeEquivalentTo(new { erro = "Não é possível cancelar uma campanha já encerrada." });
    }

    // ====================================================================
    // --- TESTES DE CICLO DE VIDA (EDITAR, ATIVAR E PRORROGAR) ---
    // ====================================================================

    [Fact(DisplayName = "Editar: Deve retornar HTTP 200 (OK) ao editar campanha com sucesso")]
    public async Task Editar_QuandoSucesso_DeveRetornarOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new EditarCampanhaCommand(id, "Novo Título", "Nova Descrição", 2000m);
        var resultadoSucesso = Result<string>.Ok("Campanha editada com sucesso.");

        _mediatorMock.Setup(m => m.Send(It.IsAny<EditarCampanhaCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(resultadoSucesso);

        // Act
        var resultado = await _controller.Editar(id, command, CancellationToken.None);

        // Assert
        var objectResult = resultado.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(200);
        objectResult.Value.Should().BeEquivalentTo(new { mensagem = "Campanha editada com sucesso." });
    }

    [Fact(DisplayName = "Ativar: Deve retornar HTTP 200 (OK) ao ativar campanha com sucesso")]
    public async Task Ativar_QuandoSucesso_DeveRetornarOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var resultadoSucesso = Result<string>.Ok("Campanha ativada com sucesso.");

        _mediatorMock.Setup(m => m.Send(It.IsAny<AtivarCampanhaCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(resultadoSucesso);

        // Act
        var resultado = await _controller.Ativar(id, CancellationToken.None);

        // Assert
        var objectResult = resultado.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(200);
        objectResult.Value.Should().BeEquivalentTo(new { mensagem = "Campanha ativada com sucesso." });
    }

    [Fact(DisplayName = "Prorrogar: Deve retornar HTTP 200 (OK) ao prorrogar campanha com sucesso")]
    public async Task Prorrogar_QuandoSucesso_DeveRetornarOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new ProrrogarCampanhaCommand(id, DateTime.UtcNow.AddDays(15));
        var resultadoSucesso = Result<string>.Ok("Campanha prorrogada com sucesso.");

        _mediatorMock.Setup(m => m.Send(It.IsAny<ProrrogarCampanhaCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(resultadoSucesso);

        // Act
        var resultado = await _controller.Prorrogar(id, command, CancellationToken.None);

        // Assert
        var objectResult = resultado.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(200);
        objectResult.Value.Should().BeEquivalentTo(new { mensagem = "Campanha prorrogada com sucesso." });
    }


}