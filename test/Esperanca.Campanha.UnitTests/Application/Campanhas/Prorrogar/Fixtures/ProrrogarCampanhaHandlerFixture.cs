using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Campanhas.Prorrogar;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Prorrogar.Fixtures;

public class ProrrogarCampanhaHandlerFixture
{
    public AppDbContextMock AppDbContextMock { get; }
    public CurrentUserMock CurrentUserMock { get; }
    public ProrrogarCampanhaHandler Handler { get; }

    public ProrrogarCampanhaHandlerFixture()
    {
        AppDbContextMock = new AppDbContextMock();
        CurrentUserMock = new CurrentUserMock();

        var logger = Substitute.For<ILogger<ProrrogarCampanhaHandler>>();
        var localizer = Substitute.For<IAppLocalizer>();
        localizer[Arg.Any<string>()].Returns(c => c.Arg<string>());

        Handler = new ProrrogarCampanhaHandler(
            logger,
            AppDbContextMock.Instance,
            CurrentUserMock.Instance,
            localizer);
    }
}
