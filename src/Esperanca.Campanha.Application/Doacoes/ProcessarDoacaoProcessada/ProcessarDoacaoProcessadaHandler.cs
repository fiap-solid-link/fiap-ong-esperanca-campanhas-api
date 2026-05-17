using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Application.Transparencia._Shared;
using Esperanca.Campanha.Domain._Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.Application.Doacoes.ProcessarDoacaoProcessada;

public sealed class ProcessarDoacaoProcessadaHandler(
    ILogger<ProcessarDoacaoProcessadaHandler> logger,
    IAppDbContext dbContext,
    ITransparenciaProjectionWriter transparenciaProjectionWriter)
    : IRequestHandler<ProcessarDoacaoProcessadaCommand, Unit>
{
    public async Task<Unit> Handle(ProcessarDoacaoProcessadaCommand command, CancellationToken ct)
    {
        var campanha = await dbContext.Set<CampanhaAgg>()
            .FirstOrDefaultAsync(c => c.Id == command.IdCampanha, ct);

        if (campanha is null)
        {
            logger.LogWarning(
                "DoacaoProcessadaEvent recebido para campanha inexistente {IdCampanha}.",
                command.IdCampanha);

            return Unit.Value;
        }

        if (!campanha.PodeConcluirPorMeta(command.ValorTotalArrecadado))
        {
            logger.LogInformation(
                "Doação {IdDoacao} processada, mas campanha {IdCampanha} ainda não atingiu a meta. ValorTotalArrecadado={ValorTotalArrecadado}, MetaFinanceira={MetaFinanceira}",
                command.IdDoacao,
                command.IdCampanha,
                command.ValorTotalArrecadado,
                campanha.MetaFinanceira);

            return Unit.Value;
        }

        try
        {
            campanha.ConcluirPorMeta(command.ValorTotalArrecadado);
        }
        catch (DomainException ex)
        {
            logger.LogWarning(
                ex,
                "Erro de domínio ao concluir campanha {IdCampanha} após doação {IdDoacao}: {Codigo}",
                command.IdCampanha,
                command.IdDoacao,
                ex.Codigo);

            throw;
        }

        await dbContext.SaveChangesAsync(ct);

        await transparenciaProjectionWriter.AtualizarStatusCampanhaAsync(
            campanha.Id,
            campanha.Status.ToString(),
            command.DataProcessamento,
            ct);

        logger.LogInformation(
            "Campanha {IdCampanha} concluída após atingir a meta. IdDoacao={IdDoacao}, ValorTotalArrecadado={ValorTotalArrecadado}, Status={Status}.",
            command.IdCampanha,
            command.IdDoacao,
            command.ValorTotalArrecadado,
            campanha.Status);

        return Unit.Value;
    }
}