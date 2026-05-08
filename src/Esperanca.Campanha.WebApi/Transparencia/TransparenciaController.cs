using Esperanca.Campanha.Application.Transparencia.ConsultarDetalheCampanha;
using Esperanca.Campanha.Application.Transparencia.ConsultarListaCampanhas;
using Esperanca.Campanha.Application.Transparencia.ConsultarPainelMacro;
using Esperanca.Campanha.WebApi._Shared.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Esperanca.Campanha.WebApi.Transparencia;

[ApiController]
[Route("api/transparencia")]
[AllowAnonymous]
public sealed class TransparenciaController(ISender sender) : ControllerBase
{
    [HttpGet("painel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterPainel(CancellationToken ct)
    {
        var result = await sender.Send(new ConsultarPainelMacroQuery(), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("campanhas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarCampanhas(CancellationToken ct)
    {
        var result = await sender.Send(new ConsultarListaCampanhasQuery(), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("campanhas/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterDetalheCampanha([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new ConsultarDetalheCampanhaQuery(id), ct);
        return result.ToActionResult(this);
    }
}
