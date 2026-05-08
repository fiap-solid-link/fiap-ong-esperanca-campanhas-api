using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Transparencia._Shared;
using MediatR;

namespace Esperanca.Campanha.Application.Transparencia.ConsultarListaCampanhas;

public sealed class ConsultarListaCampanhasHandler(ITransparenciaReadRepository repository)
    : IRequestHandler<ConsultarListaCampanhasQuery, Result<IReadOnlyList<CampanhaTransparenciaDto>>>
{
    public async Task<Result<IReadOnlyList<CampanhaTransparenciaDto>>> Handle(
        ConsultarListaCampanhasQuery query,
        CancellationToken ct)
    {
        var campanhas = await repository.ListarCampanhasAsync(ct);

        return Result<IReadOnlyList<CampanhaTransparenciaDto>>.Ok(campanhas);
    }
}
