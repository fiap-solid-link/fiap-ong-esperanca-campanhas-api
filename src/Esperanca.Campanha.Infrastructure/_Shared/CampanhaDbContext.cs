using Esperanca.Campanha.Application._Shared;
using Microsoft.EntityFrameworkCore;

namespace Esperanca.Campanha.Infrastructure._Shared;

public class CampanhaDbContext(DbContextOptions<CampanhaDbContext> options) : DbContext(options), IAppDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CampanhaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
