using Esperanca.Campanha.Application.Campanhas.Listar;
using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fakers;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas.Listar.Fakers;

public static class PaginaCampanhasDtoFaker
{
    public static PaginaCampanhasDto Valid() =>
        new([CampanhaDtoFaker.Valid()],
            Pagina: 1, TamanhoPagina: 20, TotalItens: 1, TotalPaginas: 1);
}
