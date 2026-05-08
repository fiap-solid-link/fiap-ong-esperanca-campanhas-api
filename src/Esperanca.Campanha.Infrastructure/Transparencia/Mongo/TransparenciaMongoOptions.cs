namespace Esperanca.Campanha.Infrastructure.Transparencia.Mongo;

public sealed class TransparenciaMongoOptions
{
    public const string SectionName = "TransparenciaMongo";

    public string DatabaseName { get; init; } = "doacoes_db";
    public string PainelMacroCollection { get; init; } = "painel_macro";
    public string ListaCampanhasCollection { get; init; } = "lista_campanhas";
    public string CampanhaDetalheCollection { get; init; } = "campanha_detalhe";
}
