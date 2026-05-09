using Esperanca.Campanha.Application.Transparencia._Shared;

namespace Esperanca.Campanha.UnitTests.WebApi.Transparencia._Shared.Fakers;

public static class CampanhaTransparenciaDtoFaker
{
    private static readonly DateTime _dataInicio = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime _dataFim    = new(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<CampanhaTransparenciaDto> ListaValida() =>
        [Valid()];

    public static CampanhaTransparenciaDto Valid() =>
        new(Guid.Parse("cccccccc-0000-0000-0000-000000000001"),
            "Campanha Solidária",
            MetaFinanceira: 10_000m,
            ValorArrecadado: 5_000m,
            Status: "EmAndamento",
            DataInicio: _dataInicio,
            DataFim: _dataFim,
            DataEncerramento: null);
}
