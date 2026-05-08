using Esperanca.Campanha.Application.Campanhas.Criar;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Criar.Fakers;

public static class CriarCampanhaCommandFaker
{
    private static readonly DateTime Agora = DateTimeProviderMock.DefaultNow;

    public static CriarCampanhaCommand Valid() =>
        new("Campanha de Arrecadação",
            "Descrição válida da campanha",
            Agora.AddDays(1),
            Agora.AddDays(30),
            1000m,
            ModoEncerramento.PorDataOuMeta);

    public static CriarCampanhaCommand ComDataFimNoPassado() =>
        new("Campanha de Arrecadação",
            "Descrição válida da campanha",
            Agora.AddDays(-5),
            Agora.AddDays(-1),
            1000m,
            ModoEncerramento.PorData);

    public static CriarCampanhaCommand ComTituloVazio() =>
        new("",
            "Descrição válida da campanha",
            Agora.AddDays(1),
            Agora.AddDays(30),
            1000m,
            ModoEncerramento.PorData);

    public static CriarCampanhaCommand ComDescricaoVazia() =>
        new("Campanha de Arrecadação",
            "",
            Agora.AddDays(1),
            Agora.AddDays(30),
            1000m,
            ModoEncerramento.PorData);

    public static CriarCampanhaCommand ComMetaZero() =>
        new("Campanha de Arrecadação",
            "Descrição válida da campanha",
            Agora.AddDays(1),
            Agora.AddDays(30),
            0m,
            ModoEncerramento.PorData);

    public static CriarCampanhaCommand ComMetaNegativa() =>
        new("Campanha de Arrecadação",
            "Descrição válida da campanha",
            Agora.AddDays(1),
            Agora.AddDays(30),
            -100m,
            ModoEncerramento.PorData);

    public static CriarCampanhaCommand ComDataInicioAposDataFim() =>
        new("Campanha de Arrecadação",
            "Descrição válida da campanha",
            Agora.AddDays(20),
            Agora.AddDays(10),
            1000m,
            ModoEncerramento.PorData);
}
