namespace Esperanca.Campanha.Application.Doacoes._Shared.Contracts;

public sealed record DoacaoRecebidaEvent(
    Guid IdDoacao,
    Guid IdCampanha,
    Guid IdDoador,
    decimal Valor,
    DateTime DataIntencao,
    Guid IdempotencyKey);
