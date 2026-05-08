using Esperanca.Campanha.Application.Campanhas.Ativar;
using Esperanca.Campanha.Application.Campanhas.Cancelar;
using Esperanca.Campanha.Application.Campanhas.Criar;
using Esperanca.Campanha.Application.Campanhas.Editar;
using Esperanca.Campanha.Application.Campanhas.Listar;
using Esperanca.Campanha.Application.Campanhas.Obter;
using Esperanca.Campanha.Application.Campanhas.Prorrogar;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.WebApi._Shared.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Esperanca.Campanha.WebApi.Campanhas;

[ApiController]
[Route("api/campanhas")]
[Authorize(Roles = "GestorONG")]
public sealed class CampanhaController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Criar([FromBody] CriarCampanhaCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.ToActionResult(this);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20,
        [FromQuery] StatusCampanha? status = null,
        [FromQuery] DateTime? dataInicioDe = null,
        [FromQuery] DateTime? dataInicioAte = null,
        CancellationToken ct = default)
    {
        var result = await sender.Send(
            new ListarCampanhasGestorQuery(pagina, tamanhoPagina, status, dataInicioDe, dataInicioAte),
            ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Obter([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new ObterCampanhaQuery(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Editar([FromRoute] Guid id, [FromBody] EditarCampanhaBody body, CancellationToken ct)
    {
        var result = await sender.Send(
            new EditarCampanhaCommand(id, body.Titulo, body.Descricao, body.DataInicio, body.DataFim, body.MetaFinanceira, body.ModoEncerramento),
            ct);
        return result.ToActionResult(this);
    }

    [HttpPost("{id:guid}/ativar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Ativar([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new AtivarCampanhaCommand(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPost("{id:guid}/prorrogar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Prorrogar([FromRoute] Guid id, [FromBody] ProrrogarCampanhaBody body, CancellationToken ct)
    {
        var result = await sender.Send(new ProrrogarCampanhaCommand(id, body.NovaDataFim), ct);
        return result.ToActionResult(this);
    }

    [HttpPost("{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cancelar([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new CancelarCampanhaCommand(id), ct);
        return result.ToActionResult(this);
    }
}

public record EditarCampanhaBody(
    string Titulo,
    string Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    decimal MetaFinanceira,
    ModoEncerramento ModoEncerramento);

public record ProrrogarCampanhaBody(DateTime NovaDataFim);
