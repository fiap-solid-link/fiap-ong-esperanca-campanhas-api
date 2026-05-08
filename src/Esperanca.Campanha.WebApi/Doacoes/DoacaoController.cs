using Esperanca.Campanha.Application.Doacoes.EnviarIntencao;
using Esperanca.Campanha.WebApi._Shared.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Esperanca.Campanha.WebApi.Doacoes;

[ApiController]
[Route("api/doacoes")]
[Authorize(Roles = "Doador,GestorONG")]
public sealed class DoacaoController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> EnviarIntencao(
        [FromBody] EnviarIntencaoDoacaoCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.ToActionResult(this);
    }
}
