using Esperanca.Campanha.Application.Campanhas.Editar;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Editar.Fixtures;

public class EditarCampanhaValidatorFixture
{
    public DateTimeProviderMock DateTimeProviderMock { get; }
    public EditarCampanhaValidator Validator { get; }

    public EditarCampanhaValidatorFixture()
    {
        DateTimeProviderMock = new DateTimeProviderMock();
        Validator = new EditarCampanhaValidator(DateTimeProviderMock.Instance);
    }
}
