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
        // Regras de negócio obrigatórias
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

        // Toda campanha nasce Ativa e com 0 arrecadado
        Status = StatusCampanha.Ativa;
        ValorTotalArrecadado = 0;
    }

    // Método para ser chamado pelo Consumer quando o Worker processar a doação
    public void AdicionarArrecadacao(decimal valor)
    {
        if (valor > 0)
        {
            ValorTotalArrecadado += valor;
        }
    }
}

public enum StatusCampanha
{
    Ativa,
    Concluida,
    Cancelada
}