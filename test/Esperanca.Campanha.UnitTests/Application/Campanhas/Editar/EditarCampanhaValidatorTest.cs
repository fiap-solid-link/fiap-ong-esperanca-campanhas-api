using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Editar.Fakers;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Editar.Fixtures;
using FluentValidation.TestHelper;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Editar;

public class EditarCampanhaValidatorTest
{
    [Fact]
    public void Validate_WhenValidCommand_ThenHasNoErrors()
    {
        // Arrange
        var fixture = new EditarCampanhaValidatorFixture();
        var command = EditarCampanhaCommandFaker.Valid(Guid.NewGuid());

        // Act
        var result = fixture.Validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenTituloIsEmpty_ThenHasValidationErrorForTitulo()
    {
        // Arrange
        var fixture = new EditarCampanhaValidatorFixture();
        var command = EditarCampanhaCommandFaker.ComTituloVazio(Guid.NewGuid());

        // Act
        var result = fixture.Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Titulo)
            .WithErrorMessage(CampanhaErros.TituloObrigatorio);
    }

    [Fact]
    public void Validate_WhenMetaFinanceiraIsZero_ThenHasValidationErrorForMeta()
    {
        // Arrange
        var fixture = new EditarCampanhaValidatorFixture();
        var command = EditarCampanhaCommandFaker.ComMetaZero(Guid.NewGuid());

        // Act
        var result = fixture.Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MetaFinanceira)
            .WithErrorMessage(CampanhaErros.MetaFinanceiraInvalida);
    }

    [Fact]
    public void Validate_WhenDataFimIsInPast_ThenHasValidationErrorForDataFim()
    {
        // Arrange
        var fixture = new EditarCampanhaValidatorFixture();
        var command = EditarCampanhaCommandFaker.ComDataFimNoPassado(Guid.NewGuid());

        // Act
        var result = fixture.Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DataFim)
            .WithErrorMessage(CampanhaErros.DataFimMenorQueAgora);
    }

    [Fact]
    public void Validate_WhenDataInicioIsAfterDataFim_ThenHasValidationErrorForDataInicio()
    {
        // Arrange
        var fixture = new EditarCampanhaValidatorFixture();
        var command = EditarCampanhaCommandFaker.ComDataInicioAposDataFim(Guid.NewGuid());

        // Act
        var result = fixture.Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DataInicio)
            .WithErrorMessage(CampanhaErros.DataInicioMaiorQueDataFim);
    }
}
