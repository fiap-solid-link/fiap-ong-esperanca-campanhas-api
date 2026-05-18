using Esperanca.Campanha.Domain.Campanhas;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.UnitTests.Domain.Campanhas.Fakers;

public static class CampanhaFaker
{
    public static readonly DateTime Agora = new(2026, 5, 7, 12, 0, 0, DateTimeKind.Utc);
    public static readonly Guid IdGestor = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301");

    public static CampanhaAgg Valid() =>
        ValidaComModo(ModoEncerramento.PorDataOuMeta);

    public static CampanhaAgg ValidaComTituloEDescricaoComEspacos() =>
        CampanhaAgg.Criar(
            titulo: "  Titulo com Espacos  ",
            descricao: "  Descricao com Espacos  ",
            dataInicio: Agora,
            dataFim: Agora.AddDays(30),
            metaFinanceira: 1000m,
            modoEncerramento: ModoEncerramento.PorDataOuMeta,
            idGestor: IdGestor,
            agora: Agora);

    public static Action WithInvalidTituloVazio() =>
        () => CampanhaAgg.Criar("", "Descrição válida", Agora, Agora.AddDays(30), 1000m, ModoEncerramento.PorDataOuMeta, IdGestor, Agora);

    public static Action WithInvalidTituloSoEspacos() =>
        () => CampanhaAgg.Criar("   ", "Descrição válida", Agora, Agora.AddDays(30), 1000m, ModoEncerramento.PorDataOuMeta, IdGestor, Agora);

    public static Action WithInvalidDescricao() =>
        () => CampanhaAgg.Criar("Campanha de Teste", "  ", Agora, Agora.AddDays(30), 1000m, ModoEncerramento.PorDataOuMeta, IdGestor, Agora);

    public static Action WithDataFimNoPassado() =>
        () => CampanhaAgg.Criar("Campanha de Teste", "Descrição válida", Agora, Agora.AddDays(-1), 1000m, ModoEncerramento.PorDataOuMeta, IdGestor, Agora);

    public static Action WithDataFimIgualAgora() =>
        () => CampanhaAgg.Criar("Campanha de Teste", "Descrição válida", Agora, Agora, 1000m, ModoEncerramento.PorDataOuMeta, IdGestor, Agora);

    public static Action WithDataInicioPosteriorDataFim() =>
        () => CampanhaAgg.Criar("Campanha de Teste", "Descrição válida", Agora.AddDays(20), Agora.AddDays(10), 1000m, ModoEncerramento.PorDataOuMeta, IdGestor, Agora);

    public static Action WithMetaZero() =>
        () => CampanhaAgg.Criar("Campanha de Teste", "Descrição válida", Agora, Agora.AddDays(30), 0m, ModoEncerramento.PorDataOuMeta, IdGestor, Agora);

    public static Action WithMetaNegativa() =>
        () => CampanhaAgg.Criar("Campanha de Teste", "Descrição válida", Agora, Agora.AddDays(30), -1m, ModoEncerramento.PorDataOuMeta, IdGestor, Agora);

    public static CampanhaAgg ValidaComModo(
        ModoEncerramento modo,
        DateTime? dataFim = null,
        decimal meta = 1000m) =>
        CampanhaAgg.Criar(
            titulo: "Campanha de Teste",
            descricao: "Descrição válida da campanha",
            dataInicio: Agora,
            dataFim: dataFim ?? Agora.AddDays(30),
            metaFinanceira: meta,
            modoEncerramento: modo,
            idGestor: IdGestor,
            agora: Agora);

    public static CampanhaAgg ValidaEmAndamento() =>
        ValidaEmAndamentoComModo(ModoEncerramento.PorDataOuMeta);

    public static CampanhaAgg ValidaEmAndamentoComModo(
        ModoEncerramento modo,
        DateTime? dataFim = null,
        decimal meta = 1000m)
    {
        var campanha = ValidaComModo(modo, dataFim, meta);
        campanha.Ativar();
        return campanha;
    }

    public static CampanhaAgg ValidaCadastradaComModo(
        ModoEncerramento modo,
        decimal meta = 1000m) =>
        ValidaComModo(modo, meta: meta);

    public static CampanhaAgg ValidaEmAndamentoComArrecadacao(
        ModoEncerramento modo,
        decimal valorArrecadado,
        decimal meta = 1000m)
    {
        var campanha = ValidaEmAndamentoComModo(modo, meta: meta);
        campanha.RegistrarArrecadacao(valorArrecadado);

        return campanha;
    }

    public static CampanhaAgg ValidaEmAndamentoPorMetaComArrecadacao(decimal valorArrecadado) =>
        ValidaEmAndamentoComArrecadacao(
            ModoEncerramento.PorMeta,
            valorArrecadado);

    public static CampanhaAgg ValidaEmAndamentoPorDataComArrecadacao(decimal valorArrecadado) =>
        ValidaEmAndamentoComArrecadacao(
            ModoEncerramento.PorData,
            valorArrecadado);

    public static CampanhaAgg ValidaEmAndamentoPorDataOuMetaComArrecadacao(decimal valorArrecadado) =>
        ValidaEmAndamentoComArrecadacao(
            ModoEncerramento.PorDataOuMeta,
            valorArrecadado);
}