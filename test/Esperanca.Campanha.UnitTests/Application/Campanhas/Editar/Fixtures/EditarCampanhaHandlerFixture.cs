using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Campanhas.Editar;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Editar.Fixtures;

public class EditarCampanhaHandlerFixture
{
    public AppDbContextMock AppDbContextMock { get; }
    public CurrentUserMock CurrentUserMock { get; }
    public DateTimeProviderMock DateTimeProviderMock { get; }
    public EditarCampanhaHandler Handler { get; }

    public EditarCampanhaHandlerFixture()
    {
        AppDbContextMock = new AppDbContextMock();
        CurrentUserMock = new CurrentUserMock();
        DateTimeProviderMock = new DateTimeProviderMock();

        var logger = Substitute.For<ILogger<EditarCampanhaHandler>>();
        var localizer = Substitute.For<IAppLocalizer>();
        localizer[Arg.Any<string>()].Returns(c => c.Arg<string>());

        Handler = new EditarCampanhaHandler(
            logger,
            AppDbContextMock.Instance,
            CurrentUserMock.Instance,
            DateTimeProviderMock.Instance,
            localizer);
    }
}
