using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application.Doacoes.EnviarIntencao.Fakers;
using Esperanca.Campanha.UnitTests.Application.Doacoes.EnviarIntencao.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes.EnviarIntencao;

public class EnviarIntencaoDoacaoValidatorTest
{
    [Fact]
    public void Validate_WhenComandoValido_ThenIsValid()
    {
        // Arrange
        var fixture = new EnviarIntencaoDoacaoValidatorFixture();

        // Act
        var result = fixture.Validator.Validate(EnviarIntencaoDoacaoCommandFaker.Valid(Guid.NewGuid()));

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WhenValorZero_ThenIsInvalid()
    {
        // Arrange
        var fixture = new EnviarIntencaoDoacaoValidatorFixture();

        // Act
        var result = fixture.Validator.Validate(EnviarIntencaoDoacaoCommandFaker.ComValorZero(Guid.NewGuid()));

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == CampanhaErros.ArrecadacaoExigeValorPositivo);
    }

    [Fact]
    public void Validate_WhenValorNegativo_ThenIsInvalid()
    {
        // Arrange
        var fixture = new EnviarIntencaoDoacaoValidatorFixture();

        // Act
        var result = fixture.Validator.Validate(EnviarIntencaoDoacaoCommandFaker.ComValorNegativo(Guid.NewGuid()));

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == CampanhaErros.ArrecadacaoExigeValorPositivo);
    }

    [Fact]
    public void Validate_WhenIdCampanhaVazio_ThenIsInvalid()
    {
        // Arrange
        var fixture = new EnviarIntencaoDoacaoValidatorFixture();

        // Act
        var result = fixture.Validator.Validate(EnviarIntencaoDoacaoCommandFaker.ComCampanhaVazia());

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "IdCampanha");
    }
}
