using Esperanca.Campanha.Infrastructure._Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Esperanca.Campanha.UnitTests.WebApi.Smoke;

public class CampanhaWebApplicationFactory : WebApplicationFactory<Esperanca.Campanha.WebApi.Program>
{
    public const string JwtSecretKey = "Smoke-Test-Secret-Key-Com-Mais-De-32-Caracteres-Para-HMAC-SHA256!";
    public const string JwtIssuer = "esperanca-campanha-api-tests";
    public const string JwtAudience = "esperanca-platform-tests";

    private readonly string _databaseName = $"campanha-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"]                    = JwtSecretKey,
                ["Jwt:Issuer"]                       = JwtIssuer,
                ["Jwt:Audience"]                     = JwtAudience,
                ["Jwt:AccessTokenExpirationMinutes"] = "30",
                ["ConnectionStrings:CampanhaDb"]     = "Host=localhost;Database=ignored;Username=ignored;Password=ignored"
            });
        });

        builder.ConfigureServices(services =>
        {
            ReplaceDbContextWithInMemory(services);
            RemoveExternalHealthChecks(services);
        });
    }

    private void ReplaceDbContextWithInMemory(IServiceCollection services)
    {
        var efDescriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<CampanhaDbContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType == typeof(CampanhaDbContext) ||
                (d.ServiceType.FullName?.StartsWith("Microsoft.EntityFrameworkCore") ?? false) ||
                (d.ServiceType.FullName?.StartsWith("Npgsql") ?? false))
            .ToList();

        foreach (var descriptor in efDescriptors)
            services.Remove(descriptor);

        services.AddDbContext<CampanhaDbContext>(options =>
            options.UseInMemoryDatabase(_databaseName));
    }

    private static void RemoveExternalHealthChecks(IServiceCollection services)
    {
        var registrations = services
            .Where(d => d.ServiceType == typeof(HealthCheckRegistration))
            .ToList();

        foreach (var registration in registrations)
            services.Remove(registration);
    }
}
