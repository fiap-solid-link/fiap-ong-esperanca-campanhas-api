using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Application._Shared.Localization;
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

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<CampanhaDbContext>());

        // HTTP context + usuário autenticado
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserAccessor>();

        // Utilitários
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IAppLocalizer, ResourceAppLocalizer>();

        return services;
    }
}
