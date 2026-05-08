using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Application._Shared.Localization;
using Esperanca.Campanha.Application.Doacoes._Shared;
using Esperanca.Campanha.Application.Transparencia._Shared;
using Esperanca.Campanha.Infrastructure._Shared;
using Esperanca.Campanha.Infrastructure.Campanhas.Scheduler;
using Esperanca.Campanha.Infrastructure.Doacoes.RabbitMq;
using Esperanca.Campanha.Infrastructure.Transparencia.Mongo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

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

        // Mensageria — RabbitMQ
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.AddSingleton<IDoacaoPublisher, RabbitMqDoacaoPublisher>();
        services.AddHostedService<RabbitMqDoacaoProcessadaConsumer>();

        // Scheduler de encerramento por data
        services.Configure<SchedulerOptions>(configuration.GetSection(SchedulerOptions.SectionName));
        services.AddHostedService<CampanhaSchedulerService>();

        // Transparência — MongoDB read side
        services.Configure<TransparenciaMongoOptions>(configuration.GetSection(TransparenciaMongoOptions.SectionName));
        services.AddSingleton<IMongoClient>(_ =>
            new MongoClient(configuration.GetConnectionString("DoacoesMongo")
                            ?? throw new InvalidOperationException("ConnectionStrings:DoacoesMongo é obrigatório.")));
        services.AddScoped<ITransparenciaReadRepository, TransparenciaMongoRepository>();

        return services;
    }
}
