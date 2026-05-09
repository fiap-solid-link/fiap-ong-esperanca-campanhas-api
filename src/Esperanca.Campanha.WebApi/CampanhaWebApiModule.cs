using System.Text.Json;
using Esperanca.Campanha.Application;
using Esperanca.Campanha.Infrastructure;
using Esperanca.Campanha.WebApi._Shared.Authentication;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;

namespace Esperanca.Campanha.WebApi;

public static class CampanhaWebApiModule
{
    public static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Modules
        CampanhaApplicationModule.ConfigureServices(services);
        CampanhaInfrastructureModule.ConfigureServices(services, configuration);

        services.AddHttpContextAccessor();

        // Auth
        services.AddCampanhaJwtAuthentication(configuration);

        // Controllers
        services.AddControllers();

        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title       = "Esperanca Campanha API",
                Version     = "v1",
                Description = "API de Campanhas - Plataforma Conexao Solidaria"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name         = "Authorization",
                Type         = SecuritySchemeType.Http,
                Scheme       = "bearer",
                BearerFormat = "JWT",
                In           = ParameterLocation.Header,
                Description  = "Insira o token JWT"
            });

            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", doc),
                    []
                }
            });

            options.TagActionsBy(api => api.GroupName is not null
                ? [api.GroupName]
                : api.ActionDescriptor.EndpointMetadata
                    .OfType<TagsAttribute>()
                    .SelectMany(t => t.Tags)
                    .DefaultIfEmpty("Outros")
                    .ToList());

            options.OrderActionsBy(apiDesc => apiDesc.GroupName);
        });

        // Health Checks — PostgreSQL (Mongo e RabbitMQ registrados em CampanhaInfrastructureModule)
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("CampanhaDb")!,
                name: "postgresql",
                tags: ["db", "ready"]);

        return services;
    }

    public static void MapHealthEndpoint(WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (ctx, report) =>
            {
                ctx.Response.ContentType = "application/json";
                var result = new
                {
                    status   = report.Status.ToString(),
                    duration = report.TotalDuration,
                    checks   = report.Entries.Select(e => new
                    {
                        name        = e.Key,
                        status      = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration    = e.Value.Duration,
                        tags        = e.Value.Tags
                    })
                };
                await ctx.Response.WriteAsJsonAsync(result, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
        });
    }
}
