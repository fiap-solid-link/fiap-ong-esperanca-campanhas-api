using Esperanca.Campanha.Application.Campanhas.Prorrogar;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Prorrogar.Fakers;

public static class ProrrogarCampanhaCommandFaker
{
    private static readonly DateTime Agora = DateTimeProviderMock.DefaultNow;

    public static ProrrogarCampanhaCommand Valid(Guid id) =>
        new(id, Agora.AddDays(60));

    public static ProrrogarCampanhaCommand ComNovaDataFimNoPassado(Guid id) =>
        new(id, Agora.AddDays(-1));
}
