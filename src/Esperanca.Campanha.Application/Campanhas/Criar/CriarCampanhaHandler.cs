using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Campanhas._Shared;
using Esperanca.Campanha.Domain._Shared;
using Esperanca.Campanha.Domain.Campanhas;
using MediatR;
using Microsoft.Extensions.Logging;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.Application.Campanhas.Criar;

public sealed class CriarCampanhaHandler(
    ILogger<CriarCampanhaHandler> logger,
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAppLocalizer localizer)
    : IRequestHandler<CriarCampanhaCommand, Result<CampanhaDto>>
{
    public async Task<Result<CampanhaDto>> Handle(CriarCampanhaCommand command, CancellationToken ct)
    {
        CampanhaAgg campanha;
        try
        {
            campanha = CampanhaAgg.Criar(
                command.Titulo,
                command.Descricao,
                command.DataInicio,
                command.DataFim,
                command.MetaFinanceira,
                command.ModoEncerramento,
                currentUser.UserId,
                dateTimeProvider.UtcNow);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Erro de domínio ao criar campanha: {Codigo}", ex.Codigo);
            return Result<CampanhaDto>.Fail(localizer[ex.Codigo]);
        }

        dbContext.Set<CampanhaAgg>().Add(campanha);
        await dbContext.SaveChangesAsync(ct);

        return Result<CampanhaDto>.Created(CampanhaDto.From(campanha));
    }
}
