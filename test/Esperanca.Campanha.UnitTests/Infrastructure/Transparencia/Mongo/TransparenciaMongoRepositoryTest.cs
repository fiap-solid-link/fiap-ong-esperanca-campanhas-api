using Esperanca.Campanha.Application.Transparencia._Shared;
using Esperanca.Campanha.Infrastructure.Transparencia.Mongo;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Infrastructure.Transparencia.Mongo;

public class TransparenciaMongoRepositoryTest
{
    [Fact]
    public void MapPainelMacro_WhenDocumentIsNull_ThenReturnsNull()
    {
        TransparenciaMongoRepository.MapPainelMacro(null).ShouldBeNull();
    }

    [Fact]
    public void MapPainelMacro_WhenDocumentExists_ThenMapsDto()
    {
        // Arrange
        var atualizadoEm = new DateTime(2026, 5, 17, 10, 0, 0, DateTimeKind.Utc);
        var document = new PainelMacroDocument
        {
            TotalArrecadado = 1500m,
            TotalDoacoes = 3,
            TotalCampanhasAtivas = 2,
            TotalCampanhasConcluidas = 1,
            AtualizadoEm = atualizadoEm,
            TopDoadores =
            [
                new TopDoadorDocument
                {
                    Apelido = "Doador anônimo",
                    TotalDoado = 1500m,
                    QuantidadeDoacoes = 3
                }
            ]
        };

        // Act
        var dto = TransparenciaMongoRepository.MapPainelMacro(document);

        // Assert
        dto.ShouldNotBeNull();
        dto.TotalArrecadado.ShouldBe(1500m);
        dto.TotalDoacoes.ShouldBe(3);
        dto.TotalCampanhasAtivas.ShouldBe(2);
        dto.TotalCampanhasConcluidas.ShouldBe(1);
        dto.AtualizadoEm.ShouldBe(atualizadoEm);
        dto.TopDoadores.Single().Apelido.ShouldBe("Doador anônimo");
    }

    [Fact]
    public void MapListaCampanhas_ThenOrdersEmAndamentoFirstAndMapsFields()
    {
        // Arrange
        var primeira = Guid.NewGuid();
        var segunda = Guid.NewGuid();
        var docs = new[]
        {
            new CampanhaListaDocument
            {
                IdCampanha = primeira,
                Titulo = "Concluida",
                MetaFinanceira = 100m,
                ValorArrecadado = 100m,
                Status = "Concluida",
                DataInicio = new DateTime(2026, 1, 1),
                DataFim = new DateTime(2026, 1, 10),
                DataEncerramento = new DateTime(2026, 1, 9)
            },
            new CampanhaListaDocument
            {
                IdCampanha = segunda,
                Titulo = "Em andamento",
                MetaFinanceira = 200m,
                ValorArrecadado = 50m,
                Status = "EmAndamento",
                DataInicio = new DateTime(2026, 1, 2),
                DataFim = new DateTime(2026, 1, 20),
                DataEncerramento = null
            }
        };

        // Act
        var result = TransparenciaMongoRepository.MapListaCampanhas(docs);

        // Assert
        result.Count.ShouldBe(2);
        result[0].Id.ShouldBe(segunda);
        result[0].Status.ShouldBe("EmAndamento");
        result[1].Id.ShouldBe(primeira);
    }

    [Fact]
    public void MapDetalheCampanha_WhenDocumentIsNull_ThenReturnsNull()
    {
        TransparenciaMongoRepository.MapDetalheCampanha(null).ShouldBeNull();
    }

    [Fact]
    public void MapDetalheCampanha_WhenDocumentExists_ThenMapsDto()
    {
        // Arrange
        var idCampanha = Guid.NewGuid();
        var data = new DateTime(2026, 5, 17, 23, 9, 53, DateTimeKind.Utc);
        var doc = new CampanhaDetalheDocument
        {
            IdCampanha = idCampanha,
            Titulo = "Inverno",
            Descricao = "doação de roupas",
            MetaFinanceira = 1000m,
            ValorArrecadado = 1500m,
            Status = "Concluida",
            DataInicio = data.AddDays(-1),
            DataFim = data.AddDays(10),
            DataEncerramento = data,
            Doacoes =
            [
                new DoacaoAnonimaDocument
                {
                    ApelidoDoador = "Doador anônimo",
                    Valor = 900m,
                    Data = data
                }
            ]
        };

        // Act
        var dto = TransparenciaMongoRepository.MapDetalheCampanha(doc);

        // Assert
        dto.ShouldNotBeNull();
        dto.Id.ShouldBe(idCampanha);
        dto.Titulo.ShouldBe("Inverno");
        dto.ValorArrecadado.ShouldBe(1500m);
        dto.Doacoes.Single().Valor.ShouldBe(900m);
    }

    [Fact]
    public void CriarListaDocument_ThenInitializesProjectionWithZeroValorArrecadado()
    {
        // Arrange
        var input = CriarInput();

        // Act
        var document = TransparenciaMongoRepository.CriarListaDocument(input);

        // Assert
        document.IdCampanha.ShouldBe(input.IdCampanha);
        document.Titulo.ShouldBe(input.Titulo);
        document.MetaFinanceira.ShouldBe(input.MetaFinanceira);
        document.ValorArrecadado.ShouldBe(0m);
        document.Status.ShouldBe(input.Status);
        document.DataInicio.ShouldBe(input.DataInicio);
        document.DataFim.ShouldBe(input.DataFim);
        document.DataEncerramento.ShouldBeNull();
    }

    [Fact]
    public void CriarDetalheDocument_ThenInitializesProjectionWithZeroValorArrecadadoAndEmptyDoacoes()
    {
        // Arrange
        var input = CriarInput();

        // Act
        var document = TransparenciaMongoRepository.CriarDetalheDocument(input);

        // Assert
        document.IdCampanha.ShouldBe(input.IdCampanha);
        document.Titulo.ShouldBe(input.Titulo);
        document.Descricao.ShouldBe(input.Descricao);
        document.MetaFinanceira.ShouldBe(input.MetaFinanceira);
        document.ValorArrecadado.ShouldBe(0m);
        document.Status.ShouldBe(input.Status);
        document.DataEncerramento.ShouldBeNull();
        document.Doacoes.ShouldBeEmpty();
    }

    private static CriarCampanhaProjectionInput CriarInput() =>
        new(
            Guid.NewGuid(),
            "Campanha",
            "Descrição",
            1000m,
            999m,
            "EmAndamento",
            new DateTime(2026, 5, 1),
            new DateTime(2026, 5, 30),
            new DateTime(2026, 5, 20));
}
