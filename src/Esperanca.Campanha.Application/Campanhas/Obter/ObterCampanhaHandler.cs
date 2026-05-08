using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Campanhas._Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.Application.Campanhas.Obter;

public sealed class ObterCampanhaHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IAppLocalizer localizer)
    : IRequestHandler<ObterCampanhaQuery, Result<CampanhaDto>>
{
    public async Task<Result<CampanhaDto>> Handle(ObterCampanhaQuery query, CancellationToken ct)
    {
        var campanha = await dbContext.Set<CampanhaAgg>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.Id, ct);

        if (campanha is null || campanha.IdGestor != currentUser.UserId)
            return Result<CampanhaDto>.NotFound(localizer[CampanhaErrorCodes.CampanhaNaoEncontrada]);

        return Result<CampanhaDto>.Ok(CampanhaDto.From(campanha));
    }
}
