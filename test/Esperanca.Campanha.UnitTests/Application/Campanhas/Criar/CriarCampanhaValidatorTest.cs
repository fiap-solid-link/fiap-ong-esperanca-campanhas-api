using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Criar.Fakers;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Criar.Fixtures;
using FluentValidation.TestHelper;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Criar;

public class CriarCampanhaValidatorTest
{
    [Fact]
    public void Validate_WhenValidCommand_ThenHasNoErrors()
    {
        // Arrange
        var fixture = new CriarCampanhaValidatorFixture();
        var command = CriarCampanhaCommandFaker.Valid();

        // Act
        var result = fixture.Validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenTituloIsEmpty_ThenHasValidationErrorForTitulo()
    {
        // Arrange
        var fixture = new CriarCampanhaValidatorFixture();
        var command = CriarCampanhaCommandFaker.ComTituloVazio();

        // Act
        var result = fixture.Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Titulo)
            .WithErrorMessage(CampanhaErros.TituloObrigatorio);
    }

    [Fact]
    public void Validate_WhenDescricaoIsEmpty_ThenHasValidationErrorForDescricao()
    {
        // Arrange
        var fixture = new CriarCampanhaValidatorFixture();
        var command = CriarCampanhaCommandFaker.ComDescricaoVazia();

        // Act
        var result = fixture.Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Descricao)
            .WithErrorMessage(CampanhaErros.DescricaoObrigatoria);
    }

    [Fact]
    public void Validate_WhenMetaFinanceiraIsZero_ThenHasValidationErrorForMeta()
    {
        // Arrange
        var fixture = new CriarCampanhaValidatorFixture();
        var command = CriarCampanhaCommandFaker.ComMetaZero();

        // Act
        var result = fixture.Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MetaFinanceira)
            .WithErrorMessage(CampanhaErros.MetaFinanceiraInvalida);
    }

    [Fact]
    public void Validate_WhenMetaFinanceiraIsNegative_ThenHasValidationErrorForMeta()
    {
        // Arrange
        var fixture = new CriarCampanhaValidatorFixture();
        var command = CriarCampanhaCommandFaker.ComMetaNegativa();

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
        var fixture = new CriarCampanhaValidatorFixture();
        var command = CriarCampanhaCommandFaker.ComDataFimNoPassado();

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
        var fixture = new CriarCampanhaValidatorFixture();
        var command = CriarCampanhaCommandFaker.ComDataInicioAposDataFim();

        // Act
        var result = fixture.Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DataInicio)
            .WithErrorMessage(CampanhaErros.DataInicioMaiorQueDataFim);
    }
}
