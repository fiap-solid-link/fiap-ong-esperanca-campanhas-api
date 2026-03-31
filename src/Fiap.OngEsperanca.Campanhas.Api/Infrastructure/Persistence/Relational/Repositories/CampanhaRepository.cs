using System;
using System.Threading;
using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational.Repositories;

public class CampanhaRepository(CampanhasDbContext dbContext) : ICampanhaRepository
{
    public async Task<Campanha?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Campanhas.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AdicionarAsync(Campanha campanha, CancellationToken ct = default)
    {
        await dbContext.Campanhas.AddAsync(campanha, ct);
    }

    public async Task AtualizarAsync(Campanha campanha, CancellationToken ct = default)
    {
        dbContext.Campanhas.Update(campanha);
        await Task.CompletedTask; // Update não é assíncrono no EF Core, mas mantemos a assinatura
    }
}