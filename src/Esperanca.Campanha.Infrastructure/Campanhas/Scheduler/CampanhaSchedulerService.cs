using Esperanca.Campanha.Application.Campanhas.EncerrarVencidas;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Esperanca.Campanha.Infrastructure.Campanhas.Scheduler;

public sealed class CampanhaSchedulerService(
    IOptions<SchedulerOptions> options,
    IServiceScopeFactory scopeFactory,
    ILogger<CampanhaSchedulerService> logger)
    : BackgroundService
{
    private readonly SchedulerOptions _opts = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalo = TimeSpan.FromSeconds(Math.Max(1, _opts.IntervaloEmSegundos));

        logger.LogInformation(
            "CampanhaSchedulerService iniciado (intervalo={IntervaloEmSegundos}s, proximidade={ProximidadeVencimentoEmDias}d).",
            _opts.IntervaloEmSegundos, _opts.ProximidadeVencimentoEmDias);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecutarTickAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha durante tick do CampanhaSchedulerService — seguindo para o próximo ciclo.");
            }

            try
            {
                await Task.Delay(intervalo, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task ExecutarTickAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(
            new EncerrarCampanhasVencidasCommand(_opts.ProximidadeVencimentoEmDias),
            ct);
    }
}
