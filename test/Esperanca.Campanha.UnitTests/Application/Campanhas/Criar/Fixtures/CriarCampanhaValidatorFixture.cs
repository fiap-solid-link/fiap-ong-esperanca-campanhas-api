using Esperanca.Campanha.Application.Campanhas.Criar;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Criar.Fixtures;

public class CriarCampanhaValidatorFixture
{
    public DateTimeProviderMock DateTimeProviderMock { get; }
    public CriarCampanhaValidator Validator { get; }

    public CriarCampanhaValidatorFixture()
    {
        DateTimeProviderMock = new DateTimeProviderMock();
        Validator = new CriarCampanhaValidator(DateTimeProviderMock.Instance);
    }
}
