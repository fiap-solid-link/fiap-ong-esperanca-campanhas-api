using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Listar.Fakers;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Listar.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Listar;

public class ListarCampanhasGestorValidatorTest
{
    [Fact]
    public void Validate_WhenDefaults_ThenIsValid()
    {
        // Arrange
        var fixture = new ListarCampanhasGestorValidatorFixture();

        // Act
        var result = fixture.Validator.Validate(ListarCampanhasGestorQueryFaker.Default());

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WhenPaginaZero_ThenIsInvalid()
    {
        // Arrange
        var fixture = new ListarCampanhasGestorValidatorFixture();

        // Act
        var result = fixture.Validator.Validate(ListarCampanhasGestorQueryFaker.ComPaginaInvalida());

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Pagina");
    }

    [Fact]
    public void Validate_WhenTamanhoPaginaZero_ThenIsInvalid()
    {
        // Arrange
        var fixture = new ListarCampanhasGestorValidatorFixture();

        // Act
        var result = fixture.Validator.Validate(ListarCampanhasGestorQueryFaker.ComTamanhoPaginaInvalido());

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "TamanhoPagina");
    }

    [Fact]
    public void Validate_WhenTamanhoPaginaAcimaDoLimite_ThenIsInvalid()
    {
        // Arrange
        var fixture = new ListarCampanhasGestorValidatorFixture();

        // Act
        var result = fixture.Validator.Validate(ListarCampanhasGestorQueryFaker.ComTamanhoPaginaAcimaDoLimite());

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "TamanhoPagina");
    }

    [Fact]
    public void Validate_WhenIntervaloInvertido_ThenIsInvalid()
    {
        // Arrange
        var fixture = new ListarCampanhasGestorValidatorFixture();

        // Act
        var result = fixture.Validator.Validate(ListarCampanhasGestorQueryFaker.ComIntervaloInvertido());

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == CampanhaErros.DataInicioMaiorQueDataFim);
    }

    [Fact]
    public void Validate_WhenStatusValido_ThenIsValid()
    {
        // Arrange
        var fixture = new ListarCampanhasGestorValidatorFixture();

        // Act
        var result = fixture.Validator.Validate(ListarCampanhasGestorQueryFaker.ComStatus(StatusCampanha.EmAndamento));

        // Assert
        result.IsValid.ShouldBeTrue();
    }
}
