using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Campanhas.Cancelar;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Cancelar.Fixtures;

public class CancelarCampanhaHandlerFixture
{
    public AppDbContextMock AppDbContextMock { get; }
    public CurrentUserMock CurrentUserMock { get; }
    public CancelarCampanhaHandler Handler { get; }

    public CancelarCampanhaHandlerFixture()
    {
        AppDbContextMock = new AppDbContextMock();
        CurrentUserMock = new CurrentUserMock();

        var logger = Substitute.For<ILogger<CancelarCampanhaHandler>>();
        var localizer = Substitute.For<IAppLocalizer>();
        localizer[Arg.Any<string>()].Returns(c => c.Arg<string>());

        Handler = new CancelarCampanhaHandler(
            logger,
            AppDbContextMock.Instance,
            CurrentUserMock.Instance,
            localizer);
    }
}
