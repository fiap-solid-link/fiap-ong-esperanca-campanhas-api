using Esperanca.Campanha.Application.Campanhas.Listar;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Listar.Fakers;

public static class ListarCampanhasGestorQueryFaker
{
    private static readonly DateTime Agora = DateTimeProviderMock.DefaultNow;

    public static ListarCampanhasGestorQuery Default() => new();

    public static ListarCampanhasGestorQuery ComPaginacao(int pagina, int tamanhoPagina) =>
        new(pagina, tamanhoPagina);

    public static ListarCampanhasGestorQuery ComStatus(StatusCampanha status) =>
        new(Status: status);

    public static ListarCampanhasGestorQuery ComIntervaloDataInicio(int diasAtras, int diasFrente) =>
        new(DataInicioDe: Agora.AddDays(-diasAtras), DataInicioAte: Agora.AddDays(diasFrente));

    public static ListarCampanhasGestorQuery ComPaginaInvalida() =>
        new(Pagina: 0);

    public static ListarCampanhasGestorQuery ComTamanhoPaginaInvalido() =>
        new(TamanhoPagina: 0);

    public static ListarCampanhasGestorQuery ComTamanhoPaginaAcimaDoLimite() =>
        new(TamanhoPagina: ListarCampanhasGestorValidator.TamanhoPaginaMaximo + 1);

    public static ListarCampanhasGestorQuery ComIntervaloInvertido() =>
        new(DataInicioDe: Agora.AddDays(10), DataInicioAte: Agora.AddDays(-10));
}
