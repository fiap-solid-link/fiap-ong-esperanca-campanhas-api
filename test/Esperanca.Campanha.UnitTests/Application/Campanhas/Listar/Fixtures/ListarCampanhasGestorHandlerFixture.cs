using Esperanca.Campanha.Application.Campanhas.Listar;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Listar.Fixtures;

public class ListarCampanhasGestorHandlerFixture
{
    public AppDbContextMock AppDbContextMock { get; }
    public CurrentUserMock CurrentUserMock { get; }
    public ListarCampanhasGestorHandler Handler { get; }

    public ListarCampanhasGestorHandlerFixture()
    {
        AppDbContextMock = new AppDbContextMock();
        CurrentUserMock = new CurrentUserMock();

        Handler = new ListarCampanhasGestorHandler(
            AppDbContextMock.Instance,
            CurrentUserMock.Instance);
    }
}
