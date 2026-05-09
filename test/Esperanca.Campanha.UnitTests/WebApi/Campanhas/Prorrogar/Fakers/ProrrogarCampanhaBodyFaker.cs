using Esperanca.Campanha.WebApi.Campanhas;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas.Prorrogar.Fakers;

public static class ProrrogarCampanhaBodyFaker
{
    public static readonly DateTime NovaDataFim = new(2027, 3, 31, 0, 0, 0, DateTimeKind.Utc);

    public static ProrrogarCampanhaBody Valid() => new(NovaDataFim);
}
