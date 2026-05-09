using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Doacoes._Shared;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Message.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.Application.Doacoes.EnviarIntencao;

public sealed class EnviarIntencaoDoacaoHandler(
    ILogger<EnviarIntencaoDoacaoHandler> logger,
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IDoacaoPublisher publisher,
    IAppLocalizer localizer)
    : IRequestHandler<EnviarIntencaoDoacaoCommand, Result<IntencaoDoacaoDto>>
{
    public async Task<Result<IntencaoDoacaoDto>> Handle(EnviarIntencaoDoacaoCommand command, CancellationToken ct)
    {
        var campanha = await dbContext.Set<CampanhaAgg>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == command.IdCampanha, ct);

        if (campanha is null)
            return Result<IntencaoDoacaoDto>.NotFound(localizer[CampanhaErrorCodes.CampanhaNaoEncontrada]);

        if (campanha.Status != StatusCampanha.EmAndamento)
            return Result<IntencaoDoacaoDto>.Fail(localizer[CampanhaErros.ArrecadacaoSomenteEmAndamento]);

        var idDoacao       = Guid.NewGuid();
        var idempotencyKey = Guid.NewGuid();
        var dataIntencao   = dateTimeProvider.UtcNow;

        var evento = new DoacaoRecebida(
            idDoacao,
            campanha.Id,
            currentUser.UserId,
            command.Valor,
            dataIntencao,
            idempotencyKey);

        await publisher.PublicarRecebidaAsync(evento, ct);

        logger.LogInformation(
            "Intenção de doação publicada: {IdDoacao} para campanha {IdCampanha} (idempotency={IdempotencyKey})",
            idDoacao, campanha.Id, idempotencyKey);

        return Result<IntencaoDoacaoDto>.Accepted(new IntencaoDoacaoDto(idDoacao, idempotencyKey, dataIntencao));
    }
}
