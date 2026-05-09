namespace Esperanca.Campanha.Application.Transparencia._Shared;

public record CampanhaDetalheDto(
    Guid Id,
    string Titulo,
    string Descricao,
    decimal MetaFinanceira,
    decimal ValorArrecadado,
    string Status,
    DateTime DataInicio,
    DateTime DataFim,
    DateTime? DataEncerramento,
    IReadOnlyList<DoacaoAnonimaDto> Doacoes);
