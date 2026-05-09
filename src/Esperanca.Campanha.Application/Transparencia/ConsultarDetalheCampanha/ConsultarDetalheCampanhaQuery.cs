using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Transparencia._Shared;
using MediatR;

namespace Esperanca.Campanha.Application.Transparencia.ConsultarDetalheCampanha;

public record ConsultarDetalheCampanhaQuery(Guid IdCampanha) : IRequest<Result<CampanhaDetalheDto>>;
