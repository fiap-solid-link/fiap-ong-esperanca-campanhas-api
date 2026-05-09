using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Esperanca.Campanha.Infrastructure._Shared;

public static class DatabaseMigrator
{
    public static async Task MigrateAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CampanhaDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CampanhaDbContext>>();

        var pending = await context.Database.GetPendingMigrationsAsync();
        if (!pending.Any())
        {
            logger.LogInformation("Nenhuma migration pendente.");
            return;
        }

        logger.LogInformation("Aplicando {Count} migration(s) pendente(s)...", pending.Count());
        await context.Database.MigrateAsync();
        logger.LogInformation("Migrations aplicadas com sucesso.");
    }
}
