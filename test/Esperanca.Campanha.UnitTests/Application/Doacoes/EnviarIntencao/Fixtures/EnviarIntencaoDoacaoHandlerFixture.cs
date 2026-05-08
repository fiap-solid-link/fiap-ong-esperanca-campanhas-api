using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Doacoes.EnviarIntencao;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using Esperanca.Campanha.UnitTests.Application.Doacoes.EnviarIntencao.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes.EnviarIntencao.Fixtures;

public class EnviarIntencaoDoacaoHandlerFixture
{
    public AppDbContextMock AppDbContextMock { get; }
    public CurrentUserMock CurrentUserMock { get; }
    public DateTimeProviderMock DateTimeProviderMock { get; }
    public DoacaoPublisherMock DoacaoPublisherMock { get; }
    public EnviarIntencaoDoacaoHandler Handler { get; }

    public EnviarIntencaoDoacaoHandlerFixture()
    {
        AppDbContextMock = new AppDbContextMock();
        CurrentUserMock = new CurrentUserMock();
        DateTimeProviderMock = new DateTimeProviderMock();
        DoacaoPublisherMock = new DoacaoPublisherMock();

        var logger = Substitute.For<ILogger<EnviarIntencaoDoacaoHandler>>();
        var localizer = Substitute.For<IAppLocalizer>();
        localizer[Arg.Any<string>()].Returns(c => c.Arg<string>());

        Handler = new EnviarIntencaoDoacaoHandler(
            logger,
            AppDbContextMock.Instance,
            CurrentUserMock.Instance,
            DateTimeProviderMock.Instance,
            DoacaoPublisherMock.Instance,
            localizer);
    }
}
