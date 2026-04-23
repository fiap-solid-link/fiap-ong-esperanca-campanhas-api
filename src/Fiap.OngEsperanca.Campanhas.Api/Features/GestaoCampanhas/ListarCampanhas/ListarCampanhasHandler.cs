using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.ListarCampanhas;

public sealed class ListarCampanhasHandler(CampanhasDbContext dbContext)
    : IRequestHandler<ListarCampanhasQuery, Result<IEnumerable<CampanhaResponse>>>
{
    public async Task<Result<IEnumerable<CampanhaResponse>>> Handle(ListarCampanhasQuery request, CancellationToken ct)
    {
        // Usamos AsNoTracking() porque é só leitura (mais rápido)
        var campanhas = await dbContext.Set<Domain.Entities.Campanha>()
            .AsNoTracking()
            .Select(c => new CampanhaResponse(
                c.Id,
                c.Titulo,
                c.Descricao,
                c.MetaFinanceira,
                c.ValorTotalArrecadado,
                c.Status))
            .ToListAsync(ct);

        return Result<IEnumerable<CampanhaResponse>>.Ok(campanhas);
    }
}