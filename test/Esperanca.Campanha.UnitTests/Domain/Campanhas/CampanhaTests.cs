using Esperanca.Campanha.Domain._Shared;
using Esperanca.Campanha.Domain.Campanhas;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.UnitTests.Domain.Campanhas;

public class CampanhaTests
{
    private static readonly DateTime Agora = new(2026, 5, 7, 12, 0, 0, DateTimeKind.Utc);
    private static readonly Guid IdGestorPadrao = Guid.NewGuid();

    private static CampanhaAgg CriarCampanhaValida(
        ModoEncerramento modo = ModoEncerramento.PorDataOuMeta,
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        decimal meta = 1000m)
        => CampanhaAgg.Criar(
            titulo: "Campanha de teste",
            descricao: "Descrição válida",
            dataInicio: dataInicio ?? Agora,
            dataFim: dataFim ?? Agora.AddDays(30),
            metaFinanceira: meta,
            modoEncerramento: modo,
            idGestor: IdGestorPadrao,
            agora: Agora);

    // ===== Criar =====

    [Fact]
    public void Criar_ComDadosValidos_DeveIniciarComStatusCadastradaEArrecadacaoZero()
    {
        var campanha = CriarCampanhaValida();

        Assert.NotEqual(Guid.Empty, campanha.Id);
        Assert.Equal(StatusCampanha.Cadastrada, campanha.Status);
        Assert.Equal(0m, campanha.ValorArrecadado);
        Assert.Equal(IdGestorPadrao, campanha.IdGestor);
    }

    [Fact]
    public void Criar_DeveAparararEspacosDoTituloEDescricao()
    {
        var campanha = CampanhaAgg.Criar(
            "  Titulo  ", "  Descricao  ",
            Agora, Agora.AddDays(10), 100m, ModoEncerramento.PorData, IdGestorPadrao, Agora);

        Assert.Equal("Titulo", campanha.Titulo);
        Assert.Equal("Descricao", campanha.Descricao);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComTituloVazio_DeveLancarDomainException(string titulo)
    {
        var ex = Assert.Throws<DomainException>(() =>
            CampanhaAgg.Criar(
                titulo, "Descricao", Agora, Agora.AddDays(10), 100m,
                ModoEncerramento.PorData, IdGestorPadrao, Agora));

        Assert.Equal(CampanhaErros.TituloObrigatorio, ex.Codigo);
    }

    [Fact]
    public void Criar_ComDescricaoVazia_DeveLancarDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            CampanhaAgg.Criar(
                "Titulo", "  ", Agora, Agora.AddDays(10), 100m,
                ModoEncerramento.PorData, IdGestorPadrao, Agora));

        Assert.Equal(CampanhaErros.DescricaoObrigatoria, ex.Codigo);
    }

    [Fact]
    public void Criar_ComDataFimNoPassado_DeveLancarDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => CriarCampanhaValida(dataFim: Agora.AddDays(-1)));
        Assert.Equal(CampanhaErros.DataFimMenorQueAgora, ex.Codigo);
    }

    [Fact]
    public void Criar_ComDataFimIgualAgora_DeveLancarDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => CriarCampanhaValida(dataFim: Agora));
        Assert.Equal(CampanhaErros.DataFimMenorQueAgora, ex.Codigo);
    }

    [Fact]
    public void Criar_ComDataInicioPosteriorADataFim_DeveLancarDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            CriarCampanhaValida(
                dataInicio: Agora.AddDays(20),
                dataFim: Agora.AddDays(10)));

        Assert.Equal(CampanhaErros.DataInicioMaiorQueDataFim, ex.Codigo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Criar_ComMetaFinanceiraNaoPositiva_DeveLancarDomainException(decimal meta)
    {
        var ex = Assert.Throws<DomainException>(() => CriarCampanhaValida(meta: meta));
        Assert.Equal(CampanhaErros.MetaFinanceiraInvalida, ex.Codigo);
    }

    // ===== Editar =====

    [Fact]
    public void Editar_EmCadastrada_DeveAtualizarCampos()
    {
        var campanha = CriarCampanhaValida();

        campanha.Editar(
            titulo: "Novo titulo",
            descricao: "Nova descricao",
            dataInicio: Agora.AddDays(1),
            dataFim: Agora.AddDays(60),
            metaFinanceira: 5000m,
            modoEncerramento: ModoEncerramento.PorMeta,
            agora: Agora);

        Assert.Equal("Novo titulo", campanha.Titulo);
        Assert.Equal("Nova descricao", campanha.Descricao);
        Assert.Equal(5000m, campanha.MetaFinanceira);
        Assert.Equal(ModoEncerramento.PorMeta, campanha.ModoEncerramento);
        Assert.Equal(StatusCampanha.Cadastrada, campanha.Status);
    }

    [Fact]
    public void Editar_EmEmAndamento_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida();
        campanha.Ativar();

        var ex = Assert.Throws<DomainException>(() =>
            campanha.Editar("X", "Y", Agora, Agora.AddDays(40), 200m, ModoEncerramento.PorData, Agora));

        Assert.Equal(CampanhaErros.EdicaoSomenteEmCadastrada, ex.Codigo);
    }

    [Fact]
    public void Editar_EmCancelada_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida();
        campanha.Ativar();
        campanha.Cancelar();

        var ex = Assert.Throws<DomainException>(() =>
            campanha.Editar("X", "Y", Agora, Agora.AddDays(40), 200m, ModoEncerramento.PorData, Agora));

        Assert.Equal(CampanhaErros.EdicaoSomenteEmCadastrada, ex.Codigo);
    }

    // ===== Ativar =====

    [Fact]
    public void Ativar_EmCadastrada_DeveTransicionarParaEmAndamento()
    {
        var campanha = CriarCampanhaValida();

        campanha.Ativar();

        Assert.Equal(StatusCampanha.EmAndamento, campanha.Status);
    }

    [Fact]
    public void Ativar_DuasVezes_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida();
        campanha.Ativar();

        var ex = Assert.Throws<DomainException>(campanha.Ativar);
        Assert.Equal(CampanhaErros.AtivacaoSomenteEmCadastrada, ex.Codigo);
    }

    // ===== Prorrogar =====

    [Fact]
    public void Prorrogar_EmEmAndamentoComNovaDataMaior_DeveAtualizarDataFim()
    {
        var campanha = CriarCampanhaValida(dataFim: Agora.AddDays(10));
        campanha.Ativar();

        var novaData = Agora.AddDays(30);
        campanha.Prorrogar(novaData);

        Assert.Equal(novaData, campanha.DataFim);
        Assert.Equal(StatusCampanha.EmAndamento, campanha.Status);
    }

    [Fact]
    public void Prorrogar_EmCadastrada_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida();

        var ex = Assert.Throws<DomainException>(() => campanha.Prorrogar(Agora.AddDays(60)));
        Assert.Equal(CampanhaErros.ProrrogacaoSomenteEmAndamento, ex.Codigo);
    }

    [Fact]
    public void Prorrogar_ComNovaDataIgualOuMenor_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida(dataFim: Agora.AddDays(10));
        campanha.Ativar();

        var ex = Assert.Throws<DomainException>(() => campanha.Prorrogar(Agora.AddDays(10)));
        Assert.Equal(CampanhaErros.ProrrogacaoExigeNovaDataFimMaior, ex.Codigo);
    }

    // ===== Cancelar =====

    [Fact]
    public void Cancelar_EmEmAndamento_DeveTransicionarParaCancelada()
    {
        var campanha = CriarCampanhaValida();
        campanha.Ativar();

        campanha.Cancelar();

        Assert.Equal(StatusCampanha.Cancelada, campanha.Status);
    }

    [Fact]
    public void Cancelar_EmCadastrada_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida();

        var ex = Assert.Throws<DomainException>(campanha.Cancelar);
        Assert.Equal(CampanhaErros.CancelamentoSomenteEmAndamento, ex.Codigo);
    }

    [Fact]
    public void Cancelar_EmConcluida_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorData, dataFim: Agora.AddDays(1));
        campanha.Ativar();
        campanha.ConcluirPorData(Agora.AddDays(2));

        var ex = Assert.Throws<DomainException>(campanha.Cancelar);
        Assert.Equal(CampanhaErros.CancelamentoSomenteEmAndamento, ex.Codigo);
    }

    // ===== RegistrarArrecadacao =====

    [Fact]
    public void RegistrarArrecadacao_EmEmAndamento_DeveSomarValor()
    {
        var campanha = CriarCampanhaValida();
        campanha.Ativar();

        campanha.RegistrarArrecadacao(150m);
        campanha.RegistrarArrecadacao(50m);

        Assert.Equal(200m, campanha.ValorArrecadado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RegistrarArrecadacao_ComValorNaoPositivo_DeveLancarDomainException(decimal valor)
    {
        var campanha = CriarCampanhaValida();
        campanha.Ativar();

        var ex = Assert.Throws<DomainException>(() => campanha.RegistrarArrecadacao(valor));
        Assert.Equal(CampanhaErros.ArrecadacaoExigeValorPositivo, ex.Codigo);
    }

    [Fact]
    public void RegistrarArrecadacao_EmCadastrada_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida();

        var ex = Assert.Throws<DomainException>(() => campanha.RegistrarArrecadacao(50m));
        Assert.Equal(CampanhaErros.ArrecadacaoSomenteEmAndamento, ex.Codigo);
    }

    [Fact]
    public void RegistrarArrecadacao_AcimaDaMeta_DeveAceitar()
    {
        var campanha = CriarCampanhaValida(meta: 100m);
        campanha.Ativar();

        campanha.RegistrarArrecadacao(150m);

        Assert.Equal(150m, campanha.ValorArrecadado);
        Assert.True(campanha.AtingiuMeta);
    }

    // ===== ConcluirPorData =====

    [Theory]
    [InlineData(ModoEncerramento.PorData)]
    [InlineData(ModoEncerramento.PorDataOuMeta)]
    public void ConcluirPorData_EmEmAndamentoComDataExpiradaEModoCompativel_DeveConcluir(
        ModoEncerramento modo)
    {
        var campanha = CriarCampanhaValida(modo: modo, dataFim: Agora.AddDays(1));
        campanha.Ativar();

        campanha.ConcluirPorData(Agora.AddDays(2));

        Assert.Equal(StatusCampanha.Concluida, campanha.Status);
    }

    [Fact]
    public void ConcluirPorData_ComModoPorMeta_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorMeta, dataFim: Agora.AddDays(1));
        campanha.Ativar();

        var ex = Assert.Throws<DomainException>(() => campanha.ConcluirPorData(Agora.AddDays(2)));
        Assert.Equal(CampanhaErros.ConclusaoPorDataExigeModoCompativel, ex.Codigo);
    }

    [Fact]
    public void ConcluirPorData_ComDataFimAindaNoFuturo_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorData, dataFim: Agora.AddDays(10));
        campanha.Ativar();

        var ex = Assert.Throws<DomainException>(() => campanha.ConcluirPorData(Agora.AddDays(5)));
        Assert.Equal(CampanhaErros.ConclusaoPorDataExigeDataFimExpirada, ex.Codigo);
    }

    [Fact]
    public void ConcluirPorData_EmCadastrada_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorData, dataFim: Agora.AddDays(1));

        var ex = Assert.Throws<DomainException>(() => campanha.ConcluirPorData(Agora.AddDays(2)));
        Assert.Equal(CampanhaErros.ConclusaoSomenteEmAndamento, ex.Codigo);
    }

    // ===== ConcluirPorMeta =====

    [Theory]
    [InlineData(ModoEncerramento.PorMeta)]
    [InlineData(ModoEncerramento.PorDataOuMeta)]
    public void ConcluirPorMeta_ComMetaAtingidaEModoCompativel_DeveConcluir(ModoEncerramento modo)
    {
        var campanha = CriarCampanhaValida(modo: modo, meta: 100m);
        campanha.Ativar();
        campanha.RegistrarArrecadacao(100m);

        campanha.ConcluirPorMeta();

        Assert.Equal(StatusCampanha.Concluida, campanha.Status);
    }

    [Fact]
    public void ConcluirPorMeta_ComModoPorData_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorData, meta: 100m);
        campanha.Ativar();
        campanha.RegistrarArrecadacao(150m);

        var ex = Assert.Throws<DomainException>(campanha.ConcluirPorMeta);
        Assert.Equal(CampanhaErros.ConclusaoPorMetaExigeModoCompativel, ex.Codigo);
    }

    [Fact]
    public void ConcluirPorMeta_SemMetaAtingida_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorMeta, meta: 100m);
        campanha.Ativar();
        campanha.RegistrarArrecadacao(50m);

        var ex = Assert.Throws<DomainException>(campanha.ConcluirPorMeta);
        Assert.Equal(CampanhaErros.ConclusaoPorMetaExigeMetaAtingida, ex.Codigo);
    }

    [Fact]
    public void ConcluirPorMeta_EmCadastrada_DeveLancarDomainException()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorMeta);

        var ex = Assert.Throws<DomainException>(campanha.ConcluirPorMeta);
        Assert.Equal(CampanhaErros.ConclusaoSomenteEmAndamento, ex.Codigo);
    }

    // ===== Predicados =====

    [Fact]
    public void PodeConcluirPorData_RetornaTrue_QuandoEmAndamentoEDataExpiradaEModoCompativel()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorData, dataFim: Agora.AddDays(1));
        campanha.Ativar();

        Assert.True(campanha.PodeConcluirPorData(Agora.AddDays(2)));
    }

    [Fact]
    public void PodeConcluirPorMeta_RetornaTrue_QuandoEmAndamentoEMetaAtingidaEModoCompativel()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorMeta, meta: 100m);
        campanha.Ativar();
        campanha.RegistrarArrecadacao(100m);

        Assert.True(campanha.PodeConcluirPorMeta());
    }

    [Fact]
    public void PodeConcluirPorMeta_RetornaFalse_ComModoPorData()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorData, meta: 100m);
        campanha.Ativar();
        campanha.RegistrarArrecadacao(200m);

        Assert.False(campanha.PodeConcluirPorMeta());
    }

    // ===== EstaProximaDoVencimento =====

    [Fact]
    public void EstaProximaDoVencimento_EmEmAndamentoDentroDaJanelaEMetaNaoAtingida_DeveRetornarTrue()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorData, dataFim: Agora.AddDays(2), meta: 1000m);
        campanha.Ativar();

        Assert.True(campanha.EstaProximaDoVencimento(Agora, diasLimite: 3));
    }

    [Fact]
    public void EstaProximaDoVencimento_QuandoMetaJaAtingida_DeveRetornarFalse()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorDataOuMeta, dataFim: Agora.AddDays(2), meta: 100m);
        campanha.Ativar();
        campanha.RegistrarArrecadacao(100m);

        Assert.False(campanha.EstaProximaDoVencimento(Agora, diasLimite: 3));
    }

    [Fact]
    public void EstaProximaDoVencimento_QuandoDataFimJaExpirou_DeveRetornarFalse()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorData, dataFim: Agora.AddDays(1), meta: 1000m);
        campanha.Ativar();

        Assert.False(campanha.EstaProximaDoVencimento(Agora.AddDays(2), diasLimite: 3));
    }

    [Fact]
    public void EstaProximaDoVencimento_QuandoForaDaJanela_DeveRetornarFalse()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorData, dataFim: Agora.AddDays(10), meta: 1000m);
        campanha.Ativar();

        Assert.False(campanha.EstaProximaDoVencimento(Agora, diasLimite: 3));
    }

    [Fact]
    public void EstaProximaDoVencimento_QuandoStatusNaoEmAndamento_DeveRetornarFalse()
    {
        var campanha = CriarCampanhaValida(modo: ModoEncerramento.PorData, dataFim: Agora.AddDays(2));

        Assert.False(campanha.EstaProximaDoVencimento(Agora, diasLimite: 3));
    }
}
