using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CriarCampanha;

[ApiController]
[Route("api/campanhas")]
public class CriarCampanhaController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "GestorONG")] // Proteção exigida pelo RBAC do Hackathon
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
}