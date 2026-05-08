using Esperanca.Campanha.Application.Doacoes.EnviarIntencao;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes.EnviarIntencao.Fakers;

public static class EnviarIntencaoDoacaoCommandFaker
{
    public static EnviarIntencaoDoacaoCommand Valid(Guid idCampanha) =>
        new(idCampanha, 150m);

    public static EnviarIntencaoDoacaoCommand ComValorZero(Guid idCampanha) =>
        new(idCampanha, 0m);

    public static EnviarIntencaoDoacaoCommand ComValorNegativo(Guid idCampanha) =>
        new(idCampanha, -10m);

    public static EnviarIntencaoDoacaoCommand ComCampanhaVazia() =>
        new(Guid.Empty, 100m);
}
