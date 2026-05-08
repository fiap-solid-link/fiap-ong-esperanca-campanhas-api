namespace Esperanca.Campanha.Application.Transparencia._Shared;

public record CampanhaTransparenciaDto(
    Guid Id,
    string Titulo,
    decimal MetaFinanceira,
    decimal ValorArrecadado,
    string Status,
    DateTime DataInicio,
    DateTime DataFim,
    DateTime? DataEncerramento);
