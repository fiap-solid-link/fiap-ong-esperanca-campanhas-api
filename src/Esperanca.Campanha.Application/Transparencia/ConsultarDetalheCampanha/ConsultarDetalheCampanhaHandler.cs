using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Transparencia._Shared;
using MediatR;

namespace Esperanca.Campanha.Application.Transparencia.ConsultarDetalheCampanha;

public sealed class ConsultarDetalheCampanhaHandler(
    ITransparenciaReadRepository repository,
    IAppLocalizer localizer)
    : IRequestHandler<ConsultarDetalheCampanhaQuery, Result<CampanhaDetalheDto>>
{
    public async Task<Result<CampanhaDetalheDto>> Handle(ConsultarDetalheCampanhaQuery query, CancellationToken ct)
    {
        var detalhe = await repository.ObterDetalheCampanhaAsync(query.IdCampanha, ct);

        if (detalhe is null)
            return Result<CampanhaDetalheDto>.NotFound(localizer[CampanhaErrorCodes.CampanhaNaoEncontrada]);

        return Result<CampanhaDetalheDto>.Ok(detalhe);
    }
}
