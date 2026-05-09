using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Campanhas.Criar;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Criar.Fixtures;

public class CriarCampanhaHandlerFixture
{
    public AppDbContextMock AppDbContextMock { get; }
    public CurrentUserMock CurrentUserMock { get; }
    public DateTimeProviderMock DateTimeProviderMock { get; }
    public CriarCampanhaHandler Handler { get; }

    public CriarCampanhaHandlerFixture()
    {
        AppDbContextMock = new AppDbContextMock();
        CurrentUserMock = new CurrentUserMock();
        DateTimeProviderMock = new DateTimeProviderMock();

        var logger = Substitute.For<ILogger<CriarCampanhaHandler>>();
        var localizer = Substitute.For<IAppLocalizer>();
        localizer[Arg.Any<string>()].Returns(c => c.Arg<string>());

        Handler = new CriarCampanhaHandler(
            logger,
            AppDbContextMock.Instance,
            CurrentUserMock.Instance,
            DateTimeProviderMock.Instance,
            localizer);
    }
}
