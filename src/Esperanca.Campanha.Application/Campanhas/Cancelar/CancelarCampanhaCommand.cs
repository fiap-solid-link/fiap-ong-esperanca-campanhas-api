using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Campanhas._Shared;
using MediatR;

namespace Esperanca.Campanha.Application.Campanhas.Cancelar;

public record CancelarCampanhaCommand(Guid Id) : IRequest<Result<CampanhaDto>>;
