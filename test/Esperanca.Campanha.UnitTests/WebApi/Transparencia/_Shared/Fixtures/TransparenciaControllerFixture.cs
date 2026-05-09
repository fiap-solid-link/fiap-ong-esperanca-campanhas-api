using Esperanca.Campanha.UnitTests.WebApi.Transparencia._Shared.Mocks;
using Esperanca.Campanha.WebApi.Transparencia;

namespace Esperanca.Campanha.UnitTests.WebApi.Transparencia._Shared.Fixtures;

public class TransparenciaControllerFixture
{
    public SenderMock SenderMock { get; }
    public TransparenciaController Controller { get; }

    public TransparenciaControllerFixture()
    {
        SenderMock = new SenderMock();
        Controller = new TransparenciaController(SenderMock.Instance);
    }
}
