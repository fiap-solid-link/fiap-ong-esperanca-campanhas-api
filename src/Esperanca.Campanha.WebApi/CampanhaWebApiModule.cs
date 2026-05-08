using Esperanca.Campanha.Application;
using Esperanca.Campanha.Infrastructure;
using Esperanca.Campanha.WebApi._Shared.Authentication;
using Microsoft.OpenApi.Models;

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

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
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

        // Health Checks
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("CampanhaDb")!,
                name: "postgresql",
                tags: ["db", "ready"]);

        return services;
    }
}
