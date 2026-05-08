using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Domain._Shared;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.Domain.Doacoes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.Application.Doacoes.ProcessarDoacaoProcessada;

public sealed class ProcessarDoacaoProcessadaHandler(
    ILogger<ProcessarDoacaoProcessadaHandler> logger,
    IAppDbContext dbContext)
    : IRequestHandler<ProcessarDoacaoProcessadaCommand, Unit>
{
    public async Task<Unit> Handle(ProcessarDoacaoProcessadaCommand command, CancellationToken ct)
    {
        var jaProcessada = await dbContext.Set<ArrecadacaoProcessada>()
            .AsNoTracking()
            .AnyAsync(a => a.IdDoacao == command.IdDoacao, ct);

        if (jaProcessada)
        {
            logger.LogInformation(
                "Doação {IdDoacao} já processada anteriormente — idempotência aplicada.", command.IdDoacao);
            return Unit.Value;
        }

        var campanha = await dbContext.Set<CampanhaAgg>()
            .FirstOrDefaultAsync(c => c.Id == command.IdCampanha, ct);

        if (campanha is null)
        {
            logger.LogWarning(
                "DoacaoProcessadaEvent recebido para campanha inexistente {IdCampanha}.", command.IdCampanha);
            return Unit.Value;
        }

        try
        {
            campanha.RegistrarArrecadacao(command.Valor);

            if (campanha.PodeConcluirPorMeta())
                campanha.ConcluirPorMeta();
        }
        catch (DomainException ex)
        {
            logger.LogWarning(
                "Erro de domínio ao processar doação {IdDoacao} para campanha {IdCampanha}: {Codigo}",
                command.IdDoacao, command.IdCampanha, ex.Codigo);
            throw;
        }

        dbContext.Set<ArrecadacaoProcessada>().Add(
            ArrecadacaoProcessada.Registrar(
                command.IdDoacao,
                command.IdCampanha,
                command.Valor,
                command.DataProcessamento));

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation(
            "Doação {IdDoacao} aplicada à campanha {IdCampanha}: novo arrecadado={ValorArrecadado}, status={Status}.",
            command.IdDoacao, command.IdCampanha, campanha.ValorArrecadado, campanha.Status);

        return Unit.Value;
    }
}
