using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Campanhas._Shared;
using Esperanca.Campanha.Domain.Campanhas;
using MediatR;

namespace Esperanca.Campanha.Application.Campanhas.Editar;

public record EditarCampanhaCommand(
    Guid Id,
    string Titulo,
    string Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    decimal MetaFinanceira,
    ModoEncerramento ModoEncerramento
) : IRequest<Result<CampanhaDto>>;
