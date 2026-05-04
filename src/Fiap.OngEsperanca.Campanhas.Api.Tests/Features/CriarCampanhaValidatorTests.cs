using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CriarCampanha;
using FluentAssertions;
using Xunit;

namespace Fiap.OngEsperanca.Campanhas.Api.Tests.Features;

public class CriarCampanhaValidatorTests
{
    private readonly CriarCampanhaValidator _validator;

    public CriarCampanhaValidatorTests()
    {
        // Instanciamos o validador que será testado
        _validator = new CriarCampanhaValidator();
    }

    [Fact(DisplayName = "Deve considerar o comando válido quando todos os dados estiverem corretos")]
    public void Validar_ComandoCorreto_DeveSerValido()
    {
        // Arrange
        var comando = new CriarCampanhaCommand(
            "Campanha Crianças Felizes",
            "Arrecadação de brinquedos",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30),
            5000m);

        // Act
        var resultado = _validator.Validate(comando);

        // Assert
        resultado.IsValid.Should().BeTrue();
        resultado.Errors.Should().BeEmpty();
    }

    // O [Theory] roda o mesmo teste várias vezes, uma para cada [InlineData]
    [Theory(DisplayName = "Deve falhar quando os campos de texto ou financeiros estiverem inválidos")]
    [InlineData("", "Descrição válida", 1000)] // Título vazio
    [InlineData("Título Válido", "", 1000)]  // Descrição vazia
    [InlineData("Título Válido", "Descrição válida", 0)] // Meta zero
    [InlineData("Título Válido", "Descrição válida", -50)] // Meta negativa
    public void Validar_CamposInvalidos_DeveFalhar(string titulo, string descricao, decimal metaFinanceira)
    {
        // Arrange
        var comando = new CriarCampanhaCommand(
            titulo,
            descricao,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30),
            metaFinanceira);

        // Act
        var resultado = _validator.Validate(comando);

        // Assert
        resultado.IsValid.Should().BeFalse();
        // Garante que o FluentValidation gerou pelo menos um erro na lista
        resultado.Errors.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "Deve falhar quando a Data de Fim for anterior ou igual a Data de Início")]
    public void Validar_DataFimInvalida_DeveFalhar()
    {
        // Arrange
        var dataInicio = DateTime.UtcNow;
        var dataFim = dataInicio.AddDays(-5); // Data Fim no PASSADO!

        var comando = new CriarCampanhaCommand("Título", "Desc", dataInicio, dataFim, 1000m);

        // Act
        var resultado = _validator.Validate(comando);

        // Assert
        resultado.IsValid.Should().BeFalse();

        // Verifica especificamente se o erro apontado foi no campo DataFim
        resultado.Errors.Should().Contain(e => e.PropertyName == "DataFim");
    }
}