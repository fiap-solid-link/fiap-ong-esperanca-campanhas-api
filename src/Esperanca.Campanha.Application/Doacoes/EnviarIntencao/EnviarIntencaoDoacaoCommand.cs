using Esperanca.Campanha.Application._Shared.Results;
using MediatR;

namespace Esperanca.Campanha.Application.Doacoes.EnviarIntencao;

public record EnviarIntencaoDoacaoCommand(
    Guid IdCampanha,
    decimal Valor
) : IRequest<Result<IntencaoDoacaoDto>>;
