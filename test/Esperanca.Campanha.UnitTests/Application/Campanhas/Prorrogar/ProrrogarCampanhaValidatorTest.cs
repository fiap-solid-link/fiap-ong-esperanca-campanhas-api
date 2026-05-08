using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Prorrogar.Fakers;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Prorrogar.Fixtures;
using FluentValidation.TestHelper;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Prorrogar;

public class ProrrogarCampanhaValidatorTest
{
    [Fact]
    public void Validate_WhenValidCommand_ThenHasNoErrors()
    {
        // Arrange
        var fixture = new ProrrogarCampanhaValidatorFixture();
        var command = ProrrogarCampanhaCommandFaker.Valid(Guid.NewGuid());

        // Act
        var result = fixture.Validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenNovaDataFimIsInPast_ThenHasValidationErrorForNovaDataFim()
    {
        // Arrange
        var fixture = new ProrrogarCampanhaValidatorFixture();
        var command = ProrrogarCampanhaCommandFaker.ComNovaDataFimNoPassado(Guid.NewGuid());

        // Act
        var result = fixture.Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NovaDataFim)
            .WithErrorMessage(CampanhaErros.DataFimMenorQueAgora);
    }
}
