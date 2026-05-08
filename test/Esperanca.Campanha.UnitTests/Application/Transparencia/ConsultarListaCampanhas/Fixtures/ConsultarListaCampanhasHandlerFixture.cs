using Esperanca.Campanha.Application.Transparencia.ConsultarListaCampanhas;
using Esperanca.Campanha.UnitTests.Application.Transparencia._Shared.Mocks;

namespace Esperanca.Campanha.UnitTests.Application.Transparencia.ConsultarListaCampanhas.Fixtures;

public class ConsultarListaCampanhasHandlerFixture
{
    public TransparenciaReadRepositoryMock RepositoryMock { get; }
    public ConsultarListaCampanhasHandler Handler { get; }

    public ConsultarListaCampanhasHandlerFixture()
    {
        RepositoryMock = new TransparenciaReadRepositoryMock();
        Handler = new ConsultarListaCampanhasHandler(RepositoryMock.Instance);
    }
}
