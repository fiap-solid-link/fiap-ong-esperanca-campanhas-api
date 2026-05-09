using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Campanhas._Shared;
using MediatR;

namespace Esperanca.Campanha.Application.Campanhas.Obter;

public record ObterCampanhaQuery(Guid Id) : IRequest<Result<CampanhaDto>>;
