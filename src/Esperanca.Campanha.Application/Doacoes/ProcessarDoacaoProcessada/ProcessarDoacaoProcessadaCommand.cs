using MediatR;

namespace Esperanca.Campanha.Application.Doacoes.ProcessarDoacaoProcessada;

public record ProcessarDoacaoProcessadaCommand(
    Guid IdDoacao,
    Guid IdCampanha,
    decimal Valor,
    DateTime DataProcessamento
) : IRequest<Unit>;
