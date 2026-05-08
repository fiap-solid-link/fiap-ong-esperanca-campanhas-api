using Esperanca.Campanha.Application.Transparencia.ConsultarPainelMacro;
using Esperanca.Campanha.UnitTests.Application.Transparencia._Shared.Mocks;

namespace Esperanca.Campanha.UnitTests.Application.Transparencia.ConsultarPainelMacro.Fixtures;

public class ConsultarPainelMacroHandlerFixture
{
    public TransparenciaReadRepositoryMock RepositoryMock { get; }
    public ConsultarPainelMacroHandler Handler { get; }

    public ConsultarPainelMacroHandlerFixture()
    {
        RepositoryMock = new TransparenciaReadRepositoryMock();
        Handler = new ConsultarPainelMacroHandler(RepositoryMock.Instance);
    }
}
