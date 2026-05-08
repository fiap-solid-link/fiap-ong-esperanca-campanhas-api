using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.EncerrarVencidas.Fakers;

public static class CampanhaFaker
{
    public static readonly Guid IdGestor = CurrentUserMock.DefaultUserId;
    private static readonly DateTime Agora = DateTimeProviderMock.DefaultNow;

    public static CampanhaAgg EmAndamento(
        DateTime dataFim,
        ModoEncerramento modo = ModoEncerramento.PorDataOuMeta,
        decimal meta = 1000m,
        DateTime? criacao = null)
    {
        var instantes = criacao ?? Agora.AddDays(-90);
        var c = CampanhaAgg.Criar(
            "Campanha de Teste",
            "Descrição válida",
            instantes,
            dataFim,
            meta,
            modo,
            IdGestor,
            instantes);
        c.Ativar();
        return c;
    }

    public static CampanhaAgg Cadastrada(DateTime dataFim) =>
        CampanhaAgg.Criar(
            "Campanha Não Ativa",
            "Descrição válida",
            Agora,
            dataFim,
            1000m,
            ModoEncerramento.PorDataOuMeta,
            IdGestor,
            Agora);
}
