using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.EncerrarVencidas;

// Retorna um 'int' com a quantidade de campanhas que foram encerradas
public record EncerrarCampanhasVencidasCommand() : IRequest<Result<int>>;