using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CancelarCampanha;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.ListarCampanhas;
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

}