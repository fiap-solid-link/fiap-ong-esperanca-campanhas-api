using Esperanca.Campanha.Application.Doacoes.ProcessarDoacaoProcessada;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes.ProcessarDoacaoProcessada.Fakers;

public static class ProcessarDoacaoProcessadaCommandFaker
{
    private static readonly DateTime Agora = DateTimeProviderMock.DefaultNow;

    public static ProcessarDoacaoProcessadaCommand Valid(
        Guid idCampanha,
        decimal valor = 100m,
        decimal? valorTotalArrecadado = null,
        Guid? idDoacao = null) =>
        new(
            idDoacao ?? Guid.NewGuid(),
            idCampanha,
            valor,
            valorTotalArrecadado ?? valor,
            Agora);
}
