using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Campanhas._Shared;
using Esperanca.Campanha.Domain._Shared;
using Esperanca.Campanha.Domain.Campanhas;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.Application.Campanhas.Prorrogar;

public sealed class ProrrogarCampanhaHandler(
    ILogger<ProrrogarCampanhaHandler> logger,
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IAppLocalizer localizer)
    : IRequestHandler<ProrrogarCampanhaCommand, Result<CampanhaDto>>
{
    public async Task<Result<CampanhaDto>> Handle(ProrrogarCampanhaCommand command, CancellationToken ct)
    {
        var campanha = await dbContext.Set<CampanhaAgg>()
            .FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (campanha is null || campanha.IdGestor != currentUser.UserId)
            return Result<CampanhaDto>.NotFound(localizer[CampanhaErrorCodes.CampanhaNaoEncontrada]);

        try
        {
            campanha.Prorrogar(command.NovaDataFim);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Erro de domínio ao prorrogar campanha {Id}: {Codigo}", command.Id, ex.Codigo);
            return Result<CampanhaDto>.Fail(localizer[ex.Codigo]);
        }

        await dbContext.SaveChangesAsync(ct);

        return Result<CampanhaDto>.Ok(CampanhaDto.From(campanha));
    }
}
