using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Domain.Campanhas;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.Application.Campanhas.EncerrarVencidas;

public sealed class EncerrarCampanhasVencidasHandler(
    ILogger<EncerrarCampanhasVencidasHandler> logger,
    IAppDbContext dbContext,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<EncerrarCampanhasVencidasCommand, Unit>
{
    public async Task<Unit> Handle(EncerrarCampanhasVencidasCommand command, CancellationToken ct)
    {
        var agora = dateTimeProvider.UtcNow;

        var emAndamento = await dbContext.Set<CampanhaAgg>()
            .Where(c => c.Status == StatusCampanha.EmAndamento)
            .ToListAsync(ct);

        var encerradas = 0;

        foreach (var campanha in emAndamento)
        {
            if (campanha.PodeConcluirPorData(agora))
            {
                campanha.ConcluirPorData(agora);
                encerradas++;
                logger.LogInformation(
                    "CampanhaEncerradaPorData: {IdCampanha} (DataFim={DataFim:o})",
                    campanha.Id, campanha.DataFim);
                continue;
            }

            if (campanha.EstaProximaDoVencimento(agora, command.ProximidadeVencimentoEmDias))
            {
                logger.LogInformation(
                    "CampanhaProximaDoVencimento: {IdCampanha} (DataFim={DataFim:o}, ValorArrecadado={ValorArrecadado}, MetaFinanceira={MetaFinanceira})",
                    campanha.Id, campanha.DataFim, campanha.ValorArrecadado, campanha.MetaFinanceira);
            }
        }

        if (encerradas > 0)
            await dbContext.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
