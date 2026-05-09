using Esperanca.Campanha.Application.Doacoes.ProcessarDoacaoProcessada;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes.ProcessarDoacaoProcessada.Fixtures;

public class ProcessarDoacaoProcessadaHandlerFixture
{
    public AppDbContextMock AppDbContextMock { get; }
    public ProcessarDoacaoProcessadaHandler Handler { get; }

    public ProcessarDoacaoProcessadaHandlerFixture()
    {
        AppDbContextMock = new AppDbContextMock();
        var logger = Substitute.For<ILogger<ProcessarDoacaoProcessadaHandler>>();

        Handler = new ProcessarDoacaoProcessadaHandler(
            logger,
            AppDbContextMock.Instance);
    }
}
