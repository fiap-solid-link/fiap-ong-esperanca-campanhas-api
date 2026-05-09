using Esperanca.Campanha.Application.Transparencia._Shared;

namespace Esperanca.Campanha.UnitTests.WebApi.Transparencia._Shared.Fakers;

public static class CampanhaDetalheDtoFaker
{
    private static readonly DateTime _dataInicio = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime _dataFim    = new(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);

    public static CampanhaDetalheDto Valid() =>
        new(Guid.Parse("cccccccc-0000-0000-0000-000000000001"),
            "Campanha Solidária",
            "Ajudando quem precisa",
            MetaFinanceira: 10_000m,
            ValorArrecadado: 5_000m,
            Status: "EmAndamento",
            DataInicio: _dataInicio,
            DataFim: _dataFim,
            DataEncerramento: null,
            Doacoes: [new DoacaoAnonimaDto("Apoiador Anônimo", 100m, _dataInicio)]);
}
