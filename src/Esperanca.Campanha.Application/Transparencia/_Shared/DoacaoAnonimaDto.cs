namespace Esperanca.Campanha.Application.Transparencia._Shared;

public record DoacaoAnonimaDto(
    string ApelidoDoador,
    decimal Valor,
    DateTime Data);
