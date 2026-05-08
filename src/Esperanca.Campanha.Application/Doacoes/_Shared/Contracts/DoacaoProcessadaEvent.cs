namespace Esperanca.Campanha.Application.Doacoes._Shared.Contracts;

public sealed record DoacaoProcessadaEvent(
    Guid IdDoacao,
    Guid IdCampanha,
    decimal Valor,
    DateTime DataProcessamento);
