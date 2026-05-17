using Esperanca.Campanha.Domain._Shared;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Domain.Campanhas.Fakers;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Domain.Campanhas;

public class CampanhaTest
{
    // ===== Criar =====

    [Fact]
    public void Criar_WhenValidData_ThenInitializesWithStatusCadastradaAndZeroArrecadacao()
    {
        // Arrange & Act
        var campanha = CampanhaFaker.Valid();

        // Assert
        campanha.Id.ShouldNotBe(Guid.Empty);
        campanha.Status.ShouldBe(StatusCampanha.Cadastrada);
        campanha.ValorArrecadado.ShouldBe(0m);
        campanha.IdGestor.ShouldBe(CampanhaFaker.IdGestor);
    }

    [Fact]
    public void Criar_WhenTituloAndDescricaoHaveWhitespace_ThenTrimsBothOnCreation()
    {
        // Arrange & Act
        var campanha = CampanhaFaker.ValidaComTituloEDescricaoComEspacos();

        // Assert
        campanha.Titulo.ShouldBe("Titulo com Espacos");
        campanha.Descricao.ShouldBe("Descricao com Espacos");
    }

    [Fact]
    public void Criar_WhenTituloIsEmpty_ThenThrowsDomainException()
    {
        // Arrange
        var act = CampanhaFaker.WithInvalidTituloVazio();

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.TituloObrigatorio);
    }

    [Fact]
    public void Criar_WhenTituloIsWhitespaceOnly_ThenThrowsDomainException()
    {
        // Arrange
        var act = CampanhaFaker.WithInvalidTituloSoEspacos();

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.TituloObrigatorio);
    }

    [Fact]
    public void Criar_WhenDescricaoIsEmpty_ThenThrowsDomainException()
    {
        // Arrange
        var act = CampanhaFaker.WithInvalidDescricao();

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.DescricaoObrigatoria);
    }

    [Fact]
    public void Criar_WhenDataFimIsInThePast_ThenThrowsDomainException()
    {
        // Arrange
        var act = CampanhaFaker.WithDataFimNoPassado();

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.DataFimMenorQueAgora);
    }

    [Fact]
    public void Criar_WhenDataFimEqualsAgora_ThenThrowsDomainException()
    {
        // Arrange
        var act = CampanhaFaker.WithDataFimIgualAgora();

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.DataFimMenorQueAgora);
    }

    [Fact]
    public void Criar_WhenDataInicioIsAfterDataFim_ThenThrowsDomainException()
    {
        // Arrange
        var act = CampanhaFaker.WithDataInicioPosteriorDataFim();

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.DataInicioMaiorQueDataFim);
    }

    [Fact]
    public void Criar_WhenMetaFinanceiraIsZero_ThenThrowsDomainException()
    {
        // Arrange
        var act = CampanhaFaker.WithMetaZero();

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.MetaFinanceiraInvalida);
    }

    [Fact]
    public void Criar_WhenMetaFinanceiraIsNegative_ThenThrowsDomainException()
    {
        // Arrange
        var act = CampanhaFaker.WithMetaNegativa();

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.MetaFinanceiraInvalida);
    }

    // ===== Editar =====

    [Fact]
    public void Editar_WhenCadastrada_ThenUpdatesFields()
    {
        // Arrange
        var campanha = CampanhaFaker.Valid();

        // Act
        campanha.Editar(
            titulo: "Novo Titulo",
            descricao: "Nova Descricao",
            dataInicio: CampanhaFaker.Agora.AddDays(1),
            dataFim: CampanhaFaker.Agora.AddDays(60),
            metaFinanceira: 5000m,
            modoEncerramento: ModoEncerramento.PorMeta,
            agora: CampanhaFaker.Agora);

        // Assert
        campanha.Titulo.ShouldBe("Novo Titulo");
        campanha.Descricao.ShouldBe("Nova Descricao");
        campanha.MetaFinanceira.ShouldBe(5000m);
        campanha.ModoEncerramento.ShouldBe(ModoEncerramento.PorMeta);
        campanha.Status.ShouldBe(StatusCampanha.Cadastrada);
    }

    [Fact]
    public void Editar_WhenEmAndamento_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamento();

        // Act
        var act = () => campanha.Editar("X", "Y", CampanhaFaker.Agora, CampanhaFaker.Agora.AddDays(40), 200m, ModoEncerramento.PorData, CampanhaFaker.Agora);

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.EdicaoSomenteEmCadastrada);
    }

    [Fact]
    public void Editar_WhenCancelada_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamento();
        campanha.Cancelar();

        // Act
        var act = () => campanha.Editar("X", "Y", CampanhaFaker.Agora, CampanhaFaker.Agora.AddDays(40), 200m, ModoEncerramento.PorData, CampanhaFaker.Agora);

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.EdicaoSomenteEmCadastrada);
    }

    // ===== Ativar =====

    [Fact]
    public void Ativar_WhenCadastrada_ThenTransitionsToEmAndamento()
    {
        // Arrange
        var campanha = CampanhaFaker.Valid();

        // Act
        campanha.Ativar();

        // Assert
        campanha.Status.ShouldBe(StatusCampanha.EmAndamento);
    }

    [Fact]
    public void Ativar_WhenAlreadyEmAndamento_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamento();

        // Act
        var act = campanha.Ativar;

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.AtivacaoSomenteEmCadastrada);
    }

    // ===== Prorrogar =====

    [Fact]
    public void Prorrogar_WhenEmAndamentoAndNewDataFimIsGreater_ThenUpdatesDataFim()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorDataOuMeta, CampanhaFaker.Agora.AddDays(10));
        var novaDataFim = CampanhaFaker.Agora.AddDays(30);

        // Act
        campanha.Prorrogar(novaDataFim);

        // Assert
        campanha.DataFim.ShouldBe(novaDataFim);
        campanha.Status.ShouldBe(StatusCampanha.EmAndamento);
    }

    [Fact]
    public void Prorrogar_WhenCadastrada_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.Valid();

        // Act
        var act = () => campanha.Prorrogar(CampanhaFaker.Agora.AddDays(60));

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ProrrogacaoSomenteEmAndamento);
    }

    [Fact]
    public void Prorrogar_WhenNewDataFimEqualsCurrentDataFim_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorDataOuMeta, CampanhaFaker.Agora.AddDays(10));

        // Act
        var act = () => campanha.Prorrogar(CampanhaFaker.Agora.AddDays(10));

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ProrrogacaoExigeNovaDataFimMaior);
    }

    [Fact]
    public void Prorrogar_WhenNewDataFimIsLessThanCurrentDataFim_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorDataOuMeta, CampanhaFaker.Agora.AddDays(10));

        // Act
        var act = () => campanha.Prorrogar(CampanhaFaker.Agora.AddDays(5));

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ProrrogacaoExigeNovaDataFimMaior);
    }

    // ===== Cancelar =====

    [Fact]
    public void Cancelar_WhenEmAndamento_ThenTransitionsToCancelada()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamento();

        // Act
        campanha.Cancelar();

        // Assert
        campanha.Status.ShouldBe(StatusCampanha.Cancelada);
    }

    [Fact]
    public void Cancelar_WhenCadastrada_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.Valid();

        // Act
        var act = campanha.Cancelar;

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.CancelamentoSomenteEmAndamento);
    }

    [Fact]
    public void Cancelar_WhenConcluida_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorData, CampanhaFaker.Agora.AddDays(1));
        campanha.ConcluirPorData(CampanhaFaker.Agora.AddDays(2));

        // Act
        var act = campanha.Cancelar;

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.CancelamentoSomenteEmAndamento);
    }

    // ===== RegistrarArrecadacao =====

    [Fact]
    public void RegistrarArrecadacao_WhenEmAndamento_ThenAccumulatesValorArrecadado()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamento();

        // Act
        campanha.RegistrarArrecadacao(150m);
        campanha.RegistrarArrecadacao(50m);

        // Assert
        campanha.ValorArrecadado.ShouldBe(200m);
    }

    [Fact]
    public void RegistrarArrecadacao_WhenValorIsZero_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamento();

        // Act
        var act = () => campanha.RegistrarArrecadacao(0m);

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ArrecadacaoExigeValorPositivo);
    }

    [Fact]
    public void RegistrarArrecadacao_WhenValorIsNegative_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamento();

        // Act
        var act = () => campanha.RegistrarArrecadacao(-1m);

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ArrecadacaoExigeValorPositivo);
    }

    [Fact]
    public void RegistrarArrecadacao_WhenCadastrada_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.Valid();

        // Act
        var act = () => campanha.RegistrarArrecadacao(50m);

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ArrecadacaoSomenteEmAndamento);
    }

    [Fact]
    public void RegistrarArrecadacao_WhenValueExceedsMeta_ThenAcceptsAndSetsAtingiuMeta()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamento();

        // Act
        campanha.RegistrarArrecadacao(1500m);

        // Assert
        campanha.ValorArrecadado.ShouldBe(1500m);
        campanha.AtingiuMeta.ShouldBeTrue();
    }

    // ===== ConcluirPorData =====

    [Fact]
    public void ConcluirPorData_WhenEmAndamentoPorDataAndDataExpired_ThenConcludesSuccessfully()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorData, CampanhaFaker.Agora.AddDays(1));

        // Act
        campanha.ConcluirPorData(CampanhaFaker.Agora.AddDays(2));

        // Assert
        campanha.Status.ShouldBe(StatusCampanha.Concluida);
    }

    [Fact]
    public void ConcluirPorData_WhenEmAndamentoPorDataOuMetaAndDataExpired_ThenConcludesSuccessfully()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorDataOuMeta, CampanhaFaker.Agora.AddDays(1));

        // Act
        campanha.ConcluirPorData(CampanhaFaker.Agora.AddDays(2));

        // Assert
        campanha.Status.ShouldBe(StatusCampanha.Concluida);
    }

    [Fact]
    public void ConcluirPorData_WhenModeIsPorMeta_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorMeta, CampanhaFaker.Agora.AddDays(1));

        // Act
        var act = () => campanha.ConcluirPorData(CampanhaFaker.Agora.AddDays(2));

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ConclusaoPorDataExigeModoCompativel);
    }

    [Fact]
    public void ConcluirPorData_WhenDataFimIsInFuture_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorData, CampanhaFaker.Agora.AddDays(10));

        // Act
        var act = () => campanha.ConcluirPorData(CampanhaFaker.Agora.AddDays(5));

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ConclusaoPorDataExigeDataFimExpirada);
    }

    [Fact]
    public void ConcluirPorData_WhenCadastrada_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaComModo(ModoEncerramento.PorData, CampanhaFaker.Agora.AddDays(1));

        // Act
        var act = () => campanha.ConcluirPorData(CampanhaFaker.Agora.AddDays(2));

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ConclusaoSomenteEmAndamento);
    }

    // ===== ConcluirPorMeta =====

    [Fact]
    public void ConcluirPorMeta_WhenEmAndamentoPorMetaAndMetaReached_ThenConcludesSuccessfully()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorMeta, meta: 100m);
        campanha.RegistrarArrecadacao(100m);

        // Act
        campanha.ConcluirPorMeta();

        // Assert
        campanha.Status.ShouldBe(StatusCampanha.Concluida);
    }

    [Fact]
    public void ConcluirPorMeta_WhenEmAndamentoPorDataOuMetaAndMetaReached_ThenConcludesSuccessfully()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorDataOuMeta, meta: 100m);
        campanha.RegistrarArrecadacao(100m);

        // Act
        campanha.ConcluirPorMeta();

        // Assert
        campanha.Status.ShouldBe(StatusCampanha.Concluida);
    }

    //[Fact]
    //public void ConcluirPorMeta_WhenModeIsPorData_ThenThrowsDomainException()
    //{
    //    // Arrange
    //    var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorData, meta: 100m);
    //    campanha.RegistrarArrecadacao(150m);

    //    // Act
    //    var act = campanha.ConcluirPorMeta;

    //    // Assert
    //    Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ConclusaoPorMetaExigeModoCompativel);
    //}

    //[Fact]
    //public void ConcluirPorMeta_WhenMetaNotReached_ThenThrowsDomainException()
    //{
    //    // Arrange
    //    var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorMeta, meta: 100m);
    //    campanha.RegistrarArrecadacao(50m);

    //    // Act
    //    var act = campanha.ConcluirPorMeta;

    //    // Assert
    //    Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ConclusaoPorMetaExigeMetaAtingida);
    //}

    //[Fact]
    //public void ConcluirPorMeta_WhenCadastrada_ThenThrowsDomainException()
    //{
    //    // Arrange
    //    var campanha = CampanhaFaker.Valid();

    //    // Act
    //    var act = campanha.ConcluirPorMeta;

    //    // Assert
    //    Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ConclusaoSomenteEmAndamento);
    //}

    // ===== PodeConcluirPorData =====

    [Fact]
    public void PodeConcluirPorData_WhenEmAndamentoAndDataExpiredAndCompatibleMode_ThenReturnsTrue()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorData, CampanhaFaker.Agora.AddDays(1));

        // Act
        var result = campanha.PodeConcluirPorData(CampanhaFaker.Agora.AddDays(2));

        // Assert
        result.ShouldBeTrue();
    }

    // ===== PodeConcluirPorMeta =====

    [Fact]
    public void PodeConcluirPorMeta_WhenEmAndamentoAndMetaReachedAndCompatibleMode_ThenReturnsTrue()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorMeta, meta: 100m);
        campanha.RegistrarArrecadacao(100m);

        // Act
        var result = campanha.PodeConcluirPorMeta();

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void PodeConcluirPorMeta_WhenModeIsPorData_ThenReturnsFalse()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorData, meta: 100m);
        campanha.RegistrarArrecadacao(200m);

        // Act
        var result = campanha.PodeConcluirPorMeta();

        // Assert
        result.ShouldBeFalse();
    }

    // ===== EstaProximaDoVencimento =====

    [Fact]
    public void EstaProximaDoVencimento_WhenEmAndamentoAndWithinWindowAndMetaNotReached_ThenReturnsTrue()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorData, CampanhaFaker.Agora.AddDays(2));

        // Act
        var result = campanha.EstaProximaDoVencimento(CampanhaFaker.Agora, diasLimite: 3);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void EstaProximaDoVencimento_WhenMetaAlreadyReached_ThenReturnsFalse()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorDataOuMeta, CampanhaFaker.Agora.AddDays(2), meta: 100m);
        campanha.RegistrarArrecadacao(100m);

        // Act
        var result = campanha.EstaProximaDoVencimento(CampanhaFaker.Agora, diasLimite: 3);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void EstaProximaDoVencimento_WhenDataFimAlreadyExpired_ThenReturnsFalse()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorData, CampanhaFaker.Agora.AddDays(1));

        // Act
        var result = campanha.EstaProximaDoVencimento(CampanhaFaker.Agora.AddDays(2), diasLimite: 3);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void EstaProximaDoVencimento_WhenOutsideWindow_ThenReturnsFalse()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorData, CampanhaFaker.Agora.AddDays(10));

        // Act
        var result = campanha.EstaProximaDoVencimento(CampanhaFaker.Agora, diasLimite: 3);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void EstaProximaDoVencimento_WhenNotEmAndamento_ThenReturnsFalse()
    {
        // Arrange
        var campanha = CampanhaFaker.Valid();

        // Act
        var result = campanha.EstaProximaDoVencimento(CampanhaFaker.Agora, diasLimite: 3);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void ConcluirPorMeta_WhenValorTotalArrecadadoAtingeMeta_ThenConcludesSuccessfully()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorMeta, meta: 100m);

        // Act
        campanha.ConcluirPorMeta(100m);

        // Assert
        campanha.Status.ShouldBe(StatusCampanha.Concluida);
        campanha.ValorArrecadado.ShouldBe(0m);
    }

    [Fact]
    public void ConcluirPorMeta_WhenValorTotalArrecadadoNaoAtingeMeta_ThenThrowsDomainException()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorMeta, meta: 100m);

        // Act
        var act = () => campanha.ConcluirPorMeta(50m);

        // Assert
        Should.Throw<DomainException>(act).Codigo.ShouldBe(CampanhaErros.ConclusaoPorMetaExigeMetaAtingida);
    }

    [Fact]
    public void PodeConcluirPorMeta_WhenValorTotalArrecadadoAtingeMetaAndCompatibleMode_ThenReturnsTrue()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorMeta, meta: 100m);

        // Act
        var result = campanha.PodeConcluirPorMeta(100m);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void PodeConcluirPorMeta_WhenValorTotalArrecadadoAtingeMetaButModeIsPorData_ThenReturnsFalse()
    {
        // Arrange
        var campanha = CampanhaFaker.ValidaEmAndamentoComModo(ModoEncerramento.PorData, meta: 100m);

        // Act
        var result = campanha.PodeConcluirPorMeta(100m);

        // Assert
        result.ShouldBeFalse();
    }

}
