using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Esperanca.Campanha.Infrastructure.Transparencia.Mongo;

internal sealed class PainelMacroDocument : BaseDocument
{
    [BsonElement("totalArrecadado")]
    public decimal TotalArrecadado { get; init; }

    [BsonElement("totalDoacoes")]
    public int TotalDoacoes { get; init; }

    [BsonElement("totalCampanhasAtivas")]
    public int TotalCampanhasAtivas { get; init; }

    [BsonElement("totalCampanhasConcluidas")]
    public int TotalCampanhasConcluidas { get; init; }

    [BsonElement("topDoadores")]
    public List<TopDoadorDocument> TopDoadores { get; init; } = [];

    [BsonElement("atualizadoEm")]
    public DateTime AtualizadoEm { get; init; }
}

internal sealed class TopDoadorDocument
{
    [BsonElement("apelido")]
    public string Apelido { get; init; } = string.Empty;

    [BsonElement("totalDoado")]
    public decimal TotalDoado { get; init; }

    [BsonElement("quantidadeDoacoes")]
    public int QuantidadeDoacoes { get; init; }
}

[BsonIgnoreExtraElements]
internal sealed class CampanhaListaDocument
{
    [BsonElement("idCampanha")]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid IdCampanha { get; init; }

    [BsonElement("titulo")]
    public string Titulo { get; init; } = string.Empty;

    [BsonElement("metaFinanceira")]
    public decimal MetaFinanceira { get; init; }

    [BsonElement("valorArrecadado")]
    public decimal ValorArrecadado { get; init; }

    [BsonElement("status")]
    public string Status { get; init; } = string.Empty;

    [BsonElement("dataInicio")]
    public DateTime DataInicio { get; init; }

    [BsonElement("dataFim")]
    public DateTime DataFim { get; init; }

    [BsonElement("dataEncerramento")]
    public DateTime? DataEncerramento { get; init; }
}

[BsonIgnoreExtraElements]
internal sealed class CampanhaDetalheDocument
{
    [BsonElement("idCampanha")]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid IdCampanha { get; init; }

    [BsonElement("titulo")]
    public string Titulo { get; init; } = string.Empty;

    [BsonElement("descricao")]
    public string Descricao { get; init; } = string.Empty;

    [BsonElement("metaFinanceira")]
    public decimal MetaFinanceira { get; init; }

    [BsonElement("valorArrecadado")]
    public decimal ValorArrecadado { get; init; }

    [BsonElement("status")]
    public string Status { get; init; } = string.Empty;

    [BsonElement("dataInicio")]
    public DateTime DataInicio { get; init; }

    [BsonElement("dataFim")]
    public DateTime DataFim { get; init; }

    [BsonElement("dataEncerramento")]
    public DateTime? DataEncerramento { get; init; }

    [BsonElement("doacoes")]
    public List<DoacaoAnonimaDocument> Doacoes { get; init; } = [];
}

internal sealed class DoacaoAnonimaDocument
{
    [BsonElement("apelidoDoador")]
    public string ApelidoDoador { get; init; } = string.Empty;

    [BsonElement("valor")]
    public decimal Valor { get; init; }

    [BsonElement("data")]
    public DateTime Data { get; init; }
}
