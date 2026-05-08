using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Prorrogar.Fakers;

public static class CampanhaFaker
{
    public static readonly Guid IdGestor = CurrentUserMock.DefaultUserId;
    private static readonly DateTime Agora = DateTimeProviderMock.DefaultNow;

    public static CampanhaAgg EmCadastrada() =>
        CampanhaAgg.Criar(
            "Campanha de Teste",
            "Descrição válida da campanha",
            Agora,
            Agora.AddDays(30),
            1000m,
            ModoEncerramento.PorDataOuMeta,
            IdGestor,
            Agora);

    public static CampanhaAgg EmAndamento()
    {
        var c = EmCadastrada();
        c.Ativar();
        return c;
    }
}
