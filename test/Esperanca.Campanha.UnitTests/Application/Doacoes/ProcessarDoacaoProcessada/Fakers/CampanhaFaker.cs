using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes.ProcessarDoacaoProcessada.Fakers;

public static class CampanhaFaker
{
    public static readonly Guid IdGestor = CurrentUserMock.DefaultUserId;
    private static readonly DateTime Agora = DateTimeProviderMock.DefaultNow;

    public static CampanhaAgg EmAndamento(decimal meta = 1000m, ModoEncerramento modo = ModoEncerramento.PorDataOuMeta)
    {
        var c = CampanhaAgg.Criar(
            "Campanha de Teste",
            "Descrição válida da campanha",
            Agora,
            Agora.AddDays(30),
            meta,
            modo,
            IdGestor,
            Agora);
        c.Ativar();
        return c;
    }

    public static CampanhaAgg EmAndamentoSomentePorData(decimal meta = 1000m) =>
        EmAndamento(meta, ModoEncerramento.PorData);
}
