using Esperanca.Campanha.Application.Campanhas._Shared;
using Esperanca.Campanha.Domain.Campanhas;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fakers;

public static class CampanhaDtoFaker
{
    private static readonly DateTime _dataInicio = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime _dataFim    = new(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);

    public static CampanhaDto Valid() =>
        new(Guid.Parse("cccccccc-0000-0000-0000-000000000001"),
            "Campanha Solidária",
            "Ajudando quem precisa",
            _dataInicio,
            _dataFim,
            10_000m,
            ModoEncerramento.PorData,
            StatusCampanha.EmAndamento,
            0m,
            Guid.Parse("dddddddd-0000-0000-0000-000000000001"));
}
