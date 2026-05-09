using Esperanca.Campanha.Application.Campanhas.Criar;
using Esperanca.Campanha.Domain.Campanhas;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas.Criar.Fakers;

public static class CriarCampanhaCommandFaker
{
    private static readonly DateTime _dataInicio = new(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime _dataFim    = new(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);

    public static CriarCampanhaCommand Valid() =>
        new("Campanha Solidária", "Ajudando quem precisa",
            _dataInicio, _dataFim, 10_000m, ModoEncerramento.PorData);
}
