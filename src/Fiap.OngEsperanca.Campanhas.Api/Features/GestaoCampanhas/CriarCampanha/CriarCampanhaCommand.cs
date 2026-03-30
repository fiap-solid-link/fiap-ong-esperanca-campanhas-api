using System;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CriarCampanha;

// Atualizado para devolver o Result padrão da equipa
public record CriarCampanhaCommand(
    string Titulo,
    string Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    decimal MetaFinanceira
) : IRequest<Result<CriarCampanhaResponse>>;

// O DTO de resposta que o Controller vai enviar para o front-end
public record CriarCampanhaResponse(Guid Id, string Titulo);