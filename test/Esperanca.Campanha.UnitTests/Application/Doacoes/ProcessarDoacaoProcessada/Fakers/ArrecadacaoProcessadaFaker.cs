using Esperanca.Campanha.Domain.Doacoes;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes.ProcessarDoacaoProcessada.Fakers;

public static class ArrecadacaoProcessadaFaker
{
    public static ArrecadacaoProcessada Existente(Guid idDoacao, Guid idCampanha, decimal valor = 100m) =>
        ArrecadacaoProcessada.Registrar(
            idDoacao,
            idCampanha,
            valor,
            DateTimeProviderMock.DefaultNow);
}
