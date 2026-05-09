using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.WebApi.Campanhas;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas.Editar.Fakers;

public static class EditarCampanhaBodyFaker
{
    private static readonly DateTime _dataInicio = new(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime _dataFim    = new(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);

    public static EditarCampanhaBody Valid() =>
        new("Novo Título", "Nova Descrição",
            _dataInicio, _dataFim, 15_000m, ModoEncerramento.PorData);
}
