using Esperanca.Campanha.Application.Campanhas.EncerrarVencidas;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.EncerrarVencidas.Fixtures;

public class EncerrarCampanhasVencidasHandlerFixture
{
    public AppDbContextMock AppDbContextMock { get; }
    public DateTimeProviderMock DateTimeProviderMock { get; }
    public EncerrarCampanhasVencidasHandler Handler { get; }

    public EncerrarCampanhasVencidasHandlerFixture()
    {
        AppDbContextMock = new AppDbContextMock();
        DateTimeProviderMock = new DateTimeProviderMock();

        var logger = Substitute.For<ILogger<EncerrarCampanhasVencidasHandler>>();

        Handler = new EncerrarCampanhasVencidasHandler(
            logger,
            AppDbContextMock.Instance,
            DateTimeProviderMock.Instance);
    }
}
