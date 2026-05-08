namespace Esperanca.Campanha.Application.Transparencia._Shared;

public interface ITransparenciaReadRepository
{
    Task<PainelMacroDto?> ObterPainelMacroAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CampanhaTransparenciaDto>> ListarCampanhasAsync(CancellationToken cancellationToken = default);

    Task<CampanhaDetalheDto?> ObterDetalheCampanhaAsync(Guid idCampanha, CancellationToken cancellationToken = default);
}
