using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;

namespace Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;

public interface ICampanhaRepository
{
    Task<Campanha?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task AdicionarAsync(Campanha campanha, CancellationToken ct = default);
    Task AtualizarAsync(Campanha campanha, CancellationToken ct = default);
}