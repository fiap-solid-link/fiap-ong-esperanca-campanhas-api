using Esperanca.Campanha.Application.Doacoes.EnviarIntencao;

namespace Esperanca.Campanha.UnitTests.WebApi.Doacoes.EnviarIntencao.Fakers;

public static class EnviarIntencaoDoacaoCommandFaker
{
    public static EnviarIntencaoDoacaoCommand Valid() =>
        new(Guid.Parse("11111111-0000-0000-0000-000000000001"), 150m);
}
