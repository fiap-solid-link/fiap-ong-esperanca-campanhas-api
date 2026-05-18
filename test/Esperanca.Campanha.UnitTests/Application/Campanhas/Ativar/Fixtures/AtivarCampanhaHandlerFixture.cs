using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Campanhas.Ativar;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Ativar.Fixtures;

public class AtivarCampanhaHandlerFixture
{
    public AppDbContextMock AppDbContextMock { get; }
    public CurrentUserMock CurrentUserMock { get; }
    public TransparenciaProjectionWriterMock TransparenciaProjectionWriterMock { get; }
    public AtivarCampanhaHandler Handler { get; }

    public AtivarCampanhaHandlerFixture()
    {
        AppDbContextMock = new AppDbContextMock();
        CurrentUserMock = new CurrentUserMock();
        TransparenciaProjectionWriterMock = new TransparenciaProjectionWriterMock();

        var logger = Substitute.For<ILogger<AtivarCampanhaHandler>>();
        var localizer = Substitute.For<IAppLocalizer>();
        localizer[Arg.Any<string>()].Returns(c => c.Arg<string>());

        Handler = new AtivarCampanhaHandler(
            logger,
            AppDbContextMock.Instance,
            CurrentUserMock.Instance,            
            localizer,
            TransparenciaProjectionWriterMock.Instance);
    }
}
