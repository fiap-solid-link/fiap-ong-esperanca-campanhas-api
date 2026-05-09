using Esperanca.Campanha.Application.Campanhas._Shared;

namespace Esperanca.Campanha.Application.Campanhas.Listar;

public record PaginaCampanhasDto(
    IReadOnlyList<CampanhaDto> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalItens,
    int TotalPaginas);
