using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Campanhas._Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.Application.Campanhas.Listar;

public sealed class ListarCampanhasGestorHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser)
    : IRequestHandler<ListarCampanhasGestorQuery, Result<PaginaCampanhasDto>>
{
    public async Task<Result<PaginaCampanhasDto>> Handle(ListarCampanhasGestorQuery query, CancellationToken ct)
    {
        var consulta = dbContext.Set<CampanhaAgg>()
            .AsNoTracking()
            .Where(c => c.IdGestor == currentUser.UserId);

        if (query.Status.HasValue)
            consulta = consulta.Where(c => c.Status == query.Status.Value);

        if (query.DataInicioDe.HasValue)
            consulta = consulta.Where(c => c.DataInicio >= query.DataInicioDe.Value);

        if (query.DataInicioAte.HasValue)
            consulta = consulta.Where(c => c.DataInicio <= query.DataInicioAte.Value);

        var totalItens = await consulta.CountAsync(ct);

        var campanhas = await consulta
            .OrderByDescending(c => c.DataInicio)
            .ThenBy(c => c.Id)
            .Skip((query.Pagina - 1) * query.TamanhoPagina)
            .Take(query.TamanhoPagina)
            .ToListAsync(ct);

        var totalPaginas = totalItens == 0
            ? 0
            : (int)Math.Ceiling(totalItens / (double)query.TamanhoPagina);

        var pagina = new PaginaCampanhasDto(
            campanhas.Select(CampanhaDto.From).ToList(),
            query.Pagina,
            query.TamanhoPagina,
            totalItens,
            totalPaginas);

        return Result<PaginaCampanhasDto>.Ok(pagina);
    }
}
