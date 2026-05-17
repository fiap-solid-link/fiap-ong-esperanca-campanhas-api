namespace Esperanca.Campanha.Application.Transparencia._Shared
{
    public sealed record CriarCampanhaProjectionInput(
    Guid IdCampanha,
    string Titulo,
    string Descricao,
    decimal MetaFinanceira,
    decimal ValorArrecadado,
    string Status,
    DateTime DataInicio,
    DateTime DataFim,
    DateTime? DataEncerramento
);
}
