using System;
using System.Threading;
using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Events;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Services;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.Doacoes.EnviarIntencao;

public sealed class EnviarIntencaoDoacaoHandler(
    ICampanhaRepository campanhaRepository,
    IMessageBusService messageBus) : IRequestHandler<EnviarIntencaoDoacaoCommand, Result<string>>
{
    public async Task<Result<string>> Handle(EnviarIntencaoDoacaoCommand request, CancellationToken ct)
    {
        // 1. Valida se a campanha existe
        var campanha = await campanhaRepository.ObterPorIdAsync(request.CampanhaId, ct);
        if (campanha is null)
            return Result<string>.Fail("Campanha não encontrada.", 404);

        // 2. Valida regras de negócio (Só doa para campanha Em Andamento)
        if (campanha.Status != StatusCampanha.EmAndamento)
            return Result<string>.Fail("Não é possível doar para uma campanha que não esteja em andamento.");

        if (request.Valor <= 0)
            return Result<string>.Fail("O valor da doação deve ser maior que zero.");

        // 3. Monta o Evento que vai trafegar pelo RabbitMQ
        var evento = new DoacaoRecebidaEvent(
            request.CampanhaId,
            request.DoadorId,
            request.Valor,
            DateTime.UtcNow
        );

        // 4. Publica na fila especificada pela arquitetura
        await messageBus.PublicarAsync(evento, "doacoes-recebidas");

        // 5. Retorna sucesso dizendo que foi para processamento
        return Result<string>.Ok("Intenção de doação enviada para processamento com sucesso.");
    }
}