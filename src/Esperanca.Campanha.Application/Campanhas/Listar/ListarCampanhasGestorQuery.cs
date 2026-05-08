using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Domain.Campanhas;
using MediatR;

namespace Esperanca.Campanha.Application.Campanhas.Listar;

public record ListarCampanhasGestorQuery(
    int Pagina = 1,
    int TamanhoPagina = 20,
    StatusCampanha? Status = null,
    DateTime? DataInicioDe = null,
    DateTime? DataInicioAte = null
) : IRequest<Result<PaginaCampanhasDto>>;
