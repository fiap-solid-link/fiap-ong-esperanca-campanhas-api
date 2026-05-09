using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Transparencia.ConsultarDetalheCampanha;
using Esperanca.Campanha.UnitTests.Application.Transparencia._Shared.Mocks;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application.Transparencia.ConsultarDetalheCampanha.Fixtures;

public class ConsultarDetalheCampanhaHandlerFixture
{
    public TransparenciaReadRepositoryMock RepositoryMock { get; }
    public ConsultarDetalheCampanhaHandler Handler { get; }

    public ConsultarDetalheCampanhaHandlerFixture()
    {
        RepositoryMock = new TransparenciaReadRepositoryMock();
        var localizer = Substitute.For<IAppLocalizer>();
        localizer[Arg.Any<string>()].Returns(c => c.Arg<string>());

        Handler = new ConsultarDetalheCampanhaHandler(RepositoryMock.Instance, localizer);
    }
}
