using Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Mocks;
using Esperanca.Campanha.WebApi.Campanhas;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Fixtures;

public class CampanhaControllerFixture
{
    public SenderMock SenderMock { get; }
    public CampanhaController Controller { get; }

    public CampanhaControllerFixture()
    {
        SenderMock = new SenderMock();
        Controller = new CampanhaController(SenderMock.Instance);
    }
}
