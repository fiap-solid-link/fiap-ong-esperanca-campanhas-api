using Esperanca.Campanha.Application.Transparencia._Shared;

namespace Esperanca.Campanha.UnitTests.WebApi.Transparencia._Shared.Fakers;

public static class PainelMacroDtoFaker
{
    private static readonly DateTime _atualizadoEm = new(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);

    public static PainelMacroDto Valid() =>
        new(TotalArrecadado: 50_000m,
            TotalDoacoes: 120,
            TotalCampanhasAtivas: 3,
            TotalCampanhasConcluidas: 7,
            TopDoadores: [new TopDoadorDto("Apoiador Anônimo", 5_000m, 10)],
            AtualizadoEm: _atualizadoEm);
}
