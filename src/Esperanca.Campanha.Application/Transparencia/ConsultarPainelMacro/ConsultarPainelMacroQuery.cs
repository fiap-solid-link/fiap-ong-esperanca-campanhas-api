using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Transparencia._Shared;
using MediatR;

namespace Esperanca.Campanha.Application.Transparencia.ConsultarPainelMacro;

public record ConsultarPainelMacroQuery() : IRequest<Result<PainelMacroDto>>;
