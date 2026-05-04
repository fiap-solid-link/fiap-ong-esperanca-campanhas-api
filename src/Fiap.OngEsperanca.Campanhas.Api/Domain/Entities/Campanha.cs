using Fiap.OngEsperanca.Campanhas.Api.Domain.Exceptions;
using System;

namespace Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;

public class Campanha
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; }
    public string Descricao { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime DataFim { get; private set; }
    public decimal MetaFinanceira { get; private set; }
    public decimal ValorTotalArrecadado { get; private set; }
    public StatusCampanha Status { get; private set; }

    // Construtor vazio exigido pelo Entity Framework Core
    protected Campanha()
    {
        Titulo = null!;
        Descricao = null!;
    }

    public Campanha(string titulo, string descricao, DateTime dataInicio, DateTime dataFim, decimal metaFinanceira)
    {
        if (dataFim < DateTime.UtcNow)
            throw new DomainException("A data de término não pode estar no passado.");

        if (metaFinanceira <= 0)
            throw new DomainException("A meta financeira deve ser maior que zero.");

        Id = Guid.NewGuid();
        Titulo = titulo;
        Descricao = descricao;
        DataInicio = dataInicio;
        DataFim = dataFim;
        MetaFinanceira = metaFinanceira;

        // Alinhado com a documentação: Toda campanha nasce "Cadastrada"
        Status = StatusCampanha.Cadastrada;
        ValorTotalArrecadado = 0;
    }

    // =========================================================
    // NOVOS MÉTODOS DO CICLO DE VIDA (Event Storming)
    // =========================================================

    public void Editar(string titulo, string descricao, decimal metaFinanceira)
    {
        // Regra de Negócio: Edição só em Cadastrada
        if (Status != StatusCampanha.Cadastrada)
            throw new DomainException("Apenas campanhas no status 'Cadastrada' podem ser editadas.");

        if (metaFinanceira <= 0)
            throw new DomainException("A meta financeira deve ser maior que zero.");

        Titulo = titulo;
        Descricao = descricao;
        MetaFinanceira = metaFinanceira;
    }

    public void Ativar()
    {
        // Regra de Negócio: Transição de Cadastrada para EmAndamento
        if (Status != StatusCampanha.Cadastrada)
            throw new DomainException("Apenas campanhas 'Cadastradas' podem ser ativadas.");

        Status = StatusCampanha.EmAndamento;
    }

    public void Prorrogar(DateTime novaDataFim)
    {
        // Regra de Negócio: Prorrogação só em EmAndamento
        if (Status != StatusCampanha.EmAndamento)
            throw new DomainException("Apenas campanhas 'Em Andamento' podem ser prorrogadas.");

        if (novaDataFim <= DataFim)
            throw new DomainException("A nova data de término deve ser posterior à data atual.");

        DataFim = novaDataFim;
    }

    // =========================================================
    // MÉTODOS EXISTENTES (Ajustados)
    // =========================================================

    public void AdicionarArrecadacao(decimal valor)
    {
        if (valor > 0)
        {
            ValorTotalArrecadado += valor;
        }
    }

    public void Cancelar()
    {
        // Se já estiver Concluída ou Cancelada, não pode cancelar de novo
        if (Status == StatusCampanha.Concluida || Status == StatusCampanha.Cancelada)
            throw new DomainException("Não é possível cancelar uma campanha já encerrada ou cancelada.");

        Status = StatusCampanha.Cancelada;
    }
}

// Alinhado com o Bounded Context do projeto
public enum StatusCampanha
{
    Cadastrada,
    EmAndamento,
    Concluida,
    Cancelada
}