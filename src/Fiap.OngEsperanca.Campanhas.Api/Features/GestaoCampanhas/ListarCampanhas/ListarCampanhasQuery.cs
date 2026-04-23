using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.ListarCampanhas;

// A pergunta: "Quais são as campanhas?" (Pode receber parâmetros de paginação depois)
public record ListarCampanhasQuery() : IRequest<Result<IEnumerable<CampanhaResponse>>>;

// O formato da resposta (Nunca devolvemos a Entidade direta)
public record CampanhaResponse(
    Guid Id,
    string Titulo,
    string Descricao,
    decimal MetaFinanceira,
    decimal ValorTotalArrecadado,
    StatusCampanha Status);