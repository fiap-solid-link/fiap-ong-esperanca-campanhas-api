using Fiap.OngEsperanca.Campanhas.Api.Features.Doacoes.EnviarIntencao;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CancelarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.ListarCampanhas;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.AtivarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.EditarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.ProrrogarCampanha;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CriarCampanha;

[ApiController]
[Route("api/campanhas")]
public class CampanhasController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    //[Authorize(Roles = "GestorONG")] // Proteção exigida pelo RBAC do Hackathon
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Criar([FromBody] CriarCampanhaCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        // Padrão de retorno espelhado da identity-api
        return result.Sucesso
            ? StatusCode(result.StatusCode, result.Dados)
            : StatusCode(result.StatusCode, new { erro = result.Erro });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var query = new ListarCampanhasQuery();
        var result = await mediator.Send(query, ct);

        return result.Sucesso
            ? StatusCode(result.StatusCode, result.Dados)
            : StatusCode(result.StatusCode, new { erro = result.Erro });
    }

    [HttpPatch("{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancelar(Guid id, CancellationToken ct)
    {
        var command = new CancelarCampanhaCommand(id);
        var result = await mediator.Send(command, ct);

        return result.Sucesso
            ? StatusCode(result.StatusCode, new { mensagem = result.Dados })
            : StatusCode(result.StatusCode, new { erro = result.Erro });
    }

    [HttpPost("{id:guid}/doacoes")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Doar(Guid id, [FromBody] EnviarIntencaoDoacaoCommand command, CancellationToken ct)
    {
        // Garante que o ID da campanha na URL é o mesmo que vai pro Handler
        var comandoAtualizado = command with { CampanhaId = id };

        var result = await mediator.Send(comandoAtualizado, ct);

        return result.Sucesso
            ? StatusCode(202, new { mensagem = result.Dados })
            : StatusCode(result.StatusCode, new { erro = result.Erro });
    }

    // =========================================================
    // NOVAS ROTAS DO CICLO DE VIDA (Event Storming)
    // =========================================================

    [HttpPut("{id:guid}")]
    //[Authorize(Roles = "GestorONG")] // Proteção exigida pelo RBAC do Hackathon
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Editar(Guid id, [FromBody] EditarCampanhaCommand command, CancellationToken ct)
    {
        // Garante que o ID da URL sobrescreva o ID que veio (ou faltou) no corpo do JSON
        var comandoAtualizado = command with { Id = id };

        var result = await mediator.Send(comandoAtualizado, ct);

        return result.Sucesso
            ? StatusCode(result.StatusCode, new { mensagem = result.Dados })
            : StatusCode(result.StatusCode, new { erro = result.Erro });
    }

    [HttpPatch("{id:guid}/ativar")]
    //[Authorize(Roles = "GestorONG")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Ativar(Guid id, CancellationToken ct)
    {
        // Como o ativar não tem corpo (body), instanciamos o comando direto com o ID da URL
        var command = new AtivarCampanhaCommand(id);
        var result = await mediator.Send(command, ct);

        return result.Sucesso
            ? StatusCode(result.StatusCode, new { mensagem = result.Dados })
            : StatusCode(result.StatusCode, new { erro = result.Erro });
    }

    [HttpPatch("{id:guid}/prorrogar")]
    //[Authorize(Roles = "GestorONG")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Prorrogar(Guid id, [FromBody] ProrrogarCampanhaCommand command, CancellationToken ct)
    {
        // Pega o ID da URL e a NovaDataFim do corpo do JSON
        var comandoAtualizado = command with { Id = id };

        var result = await mediator.Send(comandoAtualizado, ct);

        return result.Sucesso
            ? StatusCode(result.StatusCode, new { mensagem = result.Dados })
            : StatusCode(result.StatusCode, new { erro = result.Erro });
    }

}