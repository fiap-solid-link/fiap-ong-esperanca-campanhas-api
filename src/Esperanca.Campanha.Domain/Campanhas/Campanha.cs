using Esperanca.Campanha.Domain._Shared;

namespace Esperanca.Campanha.Domain.Campanhas;

public class Campanha
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public DateTime DataInicio { get; private set; }
    public DateTime DataFim { get; private set; }
    public decimal MetaFinanceira { get; private set; }
    public ModoEncerramento ModoEncerramento { get; private set; }
    public StatusCampanha Status { get; private set; }
    public decimal ValorArrecadado { get; private set; }
    public Guid IdGestor { get; private set; }

    private Campanha() { }

    public static Campanha Criar(
        string titulo,
        string descricao,
        DateTime dataInicio,
        DateTime dataFim,
        decimal metaFinanceira,
        ModoEncerramento modoEncerramento,
        Guid idGestor,
        DateTime agora)
    {
        ValidarDadosCampanha(titulo, descricao, dataInicio, dataFim, metaFinanceira, agora);

        return new Campanha
        {
            Id = Guid.NewGuid(),
            Titulo = titulo.Trim(),
            Descricao = descricao.Trim(),
            DataInicio = dataInicio,
            DataFim = dataFim,
            MetaFinanceira = metaFinanceira,
            ModoEncerramento = modoEncerramento,
            Status = StatusCampanha.Cadastrada,
            ValorArrecadado = 0m,
            IdGestor = idGestor
        };
    }

    public void Editar(
        string titulo,
        string descricao,
        DateTime dataInicio,
        DateTime dataFim,
        decimal metaFinanceira,
        ModoEncerramento modoEncerramento,
        DateTime agora)
    {
        if (Status != StatusCampanha.Cadastrada)
            throw new DomainException(CampanhaErros.EdicaoSomenteEmCadastrada,
                "Edição permitida apenas em campanhas no status Cadastrada.");

        ValidarDadosCampanha(titulo, descricao, dataInicio, dataFim, metaFinanceira, agora);

        Titulo = titulo.Trim();
        Descricao = descricao.Trim();
        DataInicio = dataInicio;
        DataFim = dataFim;
        MetaFinanceira = metaFinanceira;
        ModoEncerramento = modoEncerramento;
    }

    public void Ativar()
    {
        if (Status != StatusCampanha.Cadastrada)
            throw new DomainException(CampanhaErros.AtivacaoSomenteEmCadastrada,
                "Ativação permitida apenas em campanhas no status Cadastrada.");

        Status = StatusCampanha.EmAndamento;
    }

    public void Prorrogar(DateTime novaDataFim)
    {
        if (Status != StatusCampanha.EmAndamento)
            throw new DomainException(CampanhaErros.ProrrogacaoSomenteEmAndamento,
                "Prorrogação permitida apenas em campanhas no status EmAndamento.");

        if (novaDataFim <= DataFim)
            throw new DomainException(CampanhaErros.ProrrogacaoExigeNovaDataFimMaior,
                "Nova DataFim deve ser maior que a DataFim atual.");

        DataFim = novaDataFim;
    }

    public void Cancelar()
    {
        if (Status != StatusCampanha.EmAndamento)
            throw new DomainException(CampanhaErros.CancelamentoSomenteEmAndamento,
                "Cancelamento permitido apenas em campanhas no status EmAndamento.");

        Status = StatusCampanha.Cancelada;
    }

    public void RegistrarArrecadacao(decimal valor)
    {
        if (Status != StatusCampanha.EmAndamento)
            throw new DomainException(CampanhaErros.ArrecadacaoSomenteEmAndamento,
                "Arrecadação só pode ser registrada em campanhas no status EmAndamento.");

        if (valor <= 0)
            throw new DomainException(CampanhaErros.ArrecadacaoExigeValorPositivo,
                "Valor da arrecadação deve ser positivo.");

        ValorArrecadado += valor;
    }

    public void ConcluirPorData(DateTime agora)
    {
        if (Status != StatusCampanha.EmAndamento)
            throw new DomainException(CampanhaErros.ConclusaoSomenteEmAndamento,
                "Conclusão permitida apenas em campanhas no status EmAndamento.");

        if (DataFim > agora)
            throw new DomainException(CampanhaErros.ConclusaoPorDataExigeDataFimExpirada,
                "Conclusão por data exige DataFim já expirada.");

        if (!ModoPermiteEncerramentoPorData())
            throw new DomainException(CampanhaErros.ConclusaoPorDataExigeModoCompativel,
                "Modo de encerramento da campanha não permite conclusão por data.");

        Status = StatusCampanha.Concluida;
    }

    public void ConcluirPorMeta()
    {
        if (Status != StatusCampanha.EmAndamento)
            throw new DomainException(CampanhaErros.ConclusaoSomenteEmAndamento,
                "Conclusão permitida apenas em campanhas no status EmAndamento.");

        if (!AtingiuMeta)
            throw new DomainException(CampanhaErros.ConclusaoPorMetaExigeMetaAtingida,
                "Conclusão por meta exige ValorArrecadado >= MetaFinanceira.");

        if (!ModoPermiteEncerramentoPorMeta())
            throw new DomainException(CampanhaErros.ConclusaoPorMetaExigeModoCompativel,
                "Modo de encerramento da campanha não permite conclusão por meta.");

        Status = StatusCampanha.Concluida;
    }

    public void ConcluirPorMeta(decimal valorTotalArrecadado)
    {
        if (Status != StatusCampanha.EmAndamento)
            throw new DomainException(CampanhaErros.ConclusaoSomenteEmAndamento,
                "Conclusão permitida apenas em campanhas no status EmAndamento.");

        if (!AtingiuMetaComValorTotal(valorTotalArrecadado))
            throw new DomainException(CampanhaErros.ConclusaoPorMetaExigeMetaAtingida,
                "Conclusão por meta exige ValorTotalArrecadado >= MetaFinanceira.");

        if (!ModoPermiteEncerramentoPorMeta())
            throw new DomainException(CampanhaErros.ConclusaoPorMetaExigeModoCompativel,
                "Modo de encerramento da campanha não permite conclusão por meta.");

        Status = StatusCampanha.Concluida;
    }

    public bool AtingiuMeta => ValorArrecadado >= MetaFinanceira;

    public bool AtingiuMetaComValorTotal(decimal valorTotalArrecadado) =>
        valorTotalArrecadado >= MetaFinanceira;

    public bool PodeConcluirPorData(DateTime agora) =>
        Status == StatusCampanha.EmAndamento
        && DataFim <= agora
        && ModoPermiteEncerramentoPorData();

    public bool PodeConcluirPorMeta() =>
        Status == StatusCampanha.EmAndamento
        && AtingiuMeta
        && ModoPermiteEncerramentoPorMeta();

    public bool PodeConcluirPorMeta(decimal valorTotalArrecadado) =>
        Status == StatusCampanha.EmAndamento
        && AtingiuMetaComValorTotal(valorTotalArrecadado)
        && ModoPermiteEncerramentoPorMeta();

    public bool EstaProximaDoVencimento(DateTime agora, int diasLimite)
    {
        if (Status != StatusCampanha.EmAndamento) return false;
        if (AtingiuMeta) return false;
        if (DataFim <= agora) return false;
        return (DataFim - agora).TotalDays <= diasLimite;
    }

    private bool ModoPermiteEncerramentoPorData() =>
        ModoEncerramento is ModoEncerramento.PorData or ModoEncerramento.PorDataOuMeta;

    private bool ModoPermiteEncerramentoPorMeta() =>
        ModoEncerramento is ModoEncerramento.PorMeta or ModoEncerramento.PorDataOuMeta;

    private static void ValidarDadosCampanha(
        string titulo,
        string descricao,
        DateTime dataInicio,
        DateTime dataFim,
        decimal metaFinanceira,
        DateTime agora)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException(CampanhaErros.TituloObrigatorio,
                "Título é obrigatório.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException(CampanhaErros.DescricaoObrigatoria,
                "Descrição é obrigatória.");

        if (dataFim <= agora)
            throw new DomainException(CampanhaErros.DataFimMenorQueAgora,
                "DataFim deve ser maior que a data atual.");

        if (dataInicio > dataFim)
            throw new DomainException(CampanhaErros.DataInicioMaiorQueDataFim,
                "DataInicio não pode ser posterior à DataFim.");

        if (metaFinanceira <= 0)
            throw new DomainException(CampanhaErros.MetaFinanceiraInvalida,
                "MetaFinanceira deve ser maior que zero.");
    }
}
