namespace Esperanca.Campanha.Application.Doacoes.EnviarIntencao;

public record IntencaoDoacaoDto(Guid IdDoacao, Guid IdempotencyKey, DateTime DataIntencao);
