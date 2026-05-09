using Esperanca.Campanha.UnitTests.WebApi.Doacoes.EnviarIntencao.Mocks;
using Esperanca.Campanha.WebApi.Doacoes;

namespace Esperanca.Campanha.UnitTests.WebApi.Doacoes.EnviarIntencao.Fixtures;

public class DoacaoControllerFixture
{
    public SenderMock SenderMock { get; }
    public DoacaoController Controller { get; }

    public DoacaoControllerFixture()
    {
        SenderMock = new SenderMock();
        Controller = new DoacaoController(SenderMock.Instance);
    }
}
