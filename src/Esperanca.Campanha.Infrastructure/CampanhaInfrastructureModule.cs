using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Infrastructure._Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Esperanca.Campanha.Infrastructure;

public static class CampanhaInfrastructureModule
{
    public static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // PostgreSQL + EF Core
        services.AddDbContext<CampanhaDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("CampanhaDb")));

        // Repositories
        // services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<CampanhaDbContext>());
        
        return services;
    }
}
