using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Campanhas.Obter;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Obter.Fixtures;

public class ObterCampanhaHandlerFixture
{
    public AppDbContextMock AppDbContextMock { get; }
    public CurrentUserMock CurrentUserMock { get; }
    public ObterCampanhaHandler Handler { get; }

    public ObterCampanhaHandlerFixture()
    {
        AppDbContextMock = new AppDbContextMock();
        CurrentUserMock = new CurrentUserMock();

        var localizer = Substitute.For<IAppLocalizer>();
        localizer[Arg.Any<string>()].Returns(c => c.Arg<string>());

        Handler = new ObterCampanhaHandler(
            AppDbContextMock.Instance,
            CurrentUserMock.Instance,
            localizer);
    }
}
