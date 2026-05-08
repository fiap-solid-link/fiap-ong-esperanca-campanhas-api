namespace Esperanca.Campanha.Application.Transparencia._Shared;

public record PainelMacroDto(
    decimal TotalArrecadado,
    int TotalDoacoes,
    int TotalCampanhasAtivas,
    int TotalCampanhasConcluidas,
    IReadOnlyList<TopDoadorDto> TopDoadores,
    DateTime AtualizadoEm);
