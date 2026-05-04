using System;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.EditarCampanha;

// Repare que não passamos a Data de Início e Fim, pois a regra geralmente foca em textos e metas
public record EditarCampanhaCommand(Guid Id, string Titulo, string Descricao, decimal MetaFinanceira) : IRequest<Result<string>>;