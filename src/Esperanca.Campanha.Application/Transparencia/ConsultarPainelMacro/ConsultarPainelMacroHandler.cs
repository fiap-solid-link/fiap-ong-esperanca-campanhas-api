using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Transparencia._Shared;
using MediatR;

namespace Esperanca.Campanha.Application.Transparencia.ConsultarPainelMacro;

public sealed class ConsultarPainelMacroHandler(ITransparenciaReadRepository repository)
    : IRequestHandler<ConsultarPainelMacroQuery, Result<PainelMacroDto>>
{
    public async Task<Result<PainelMacroDto>> Handle(ConsultarPainelMacroQuery query, CancellationToken ct)
    {
        var painel = await repository.ObterPainelMacroAsync(ct);

        return Result<PainelMacroDto>.Ok(painel ?? PainelMacroVazio());
    }

    private static PainelMacroDto PainelMacroVazio() =>
        new(
            TotalArrecadado: 0m,
            TotalDoacoes: 0,
            TotalCampanhasAtivas: 0,
            TotalCampanhasConcluidas: 0,
            TopDoadores: [],
            AtualizadoEm: DateTime.MinValue);
}
