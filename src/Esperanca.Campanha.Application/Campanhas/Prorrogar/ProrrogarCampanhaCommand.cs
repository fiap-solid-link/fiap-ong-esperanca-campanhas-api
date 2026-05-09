using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Campanhas._Shared;
using MediatR;

namespace Esperanca.Campanha.Application.Campanhas.Prorrogar;

public record ProrrogarCampanhaCommand(Guid Id, DateTime NovaDataFim) : IRequest<Result<CampanhaDto>>;
