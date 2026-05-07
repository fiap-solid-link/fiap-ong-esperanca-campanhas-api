using Esperanca.Campanha.Application._Shared.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Esperanca.Campanha.Application;

public static class CampanhaApplicationModule
{
    public static IServiceCollection ConfigureServices(IServiceCollection services)
    {
        var assembly = typeof(CampanhaApplicationModule).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
