using Esperanca.Campanha.Application.Campanhas.Prorrogar;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Prorrogar.Fixtures;

public class ProrrogarCampanhaValidatorFixture
{
    public DateTimeProviderMock DateTimeProviderMock { get; }
    public ProrrogarCampanhaValidator Validator { get; }

    public ProrrogarCampanhaValidatorFixture()
    {
        DateTimeProviderMock = new DateTimeProviderMock();
        Validator = new ProrrogarCampanhaValidator(DateTimeProviderMock.Instance);
    }
}
