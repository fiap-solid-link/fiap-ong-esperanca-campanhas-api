using System;
using System.Threading;
using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.EncerrarVencidas;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Schedules;

public class CampanhaVencimentoScheduler : BackgroundService
{
    private readonly ILogger<CampanhaVencimentoScheduler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public CampanhaVencimentoScheduler(
        ILogger<CampanhaVencimentoScheduler> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Scheduler] Robô de vencimento de campanhas iniciado.");

        // Fica rodando em loop enquanto a API estiver no ar
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Cria o escopo para usar o MediatR
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                // Dispara o comando
                var result = await mediator.Send(new EncerrarCampanhasVencidasCommand(), stoppingToken);

                if (result.Sucesso && result.Dados > 0)
                {
                    _logger.LogInformation($"[Scheduler] Sucesso! {result.Dados} campanhas vencidas foram encerradas.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Scheduler] Erro crítico ao verificar campanhas vencidas.");
            }

            // O Relógio: Pausa por 1 hora até rodar de novo. 
            // Dica: Troque para TimeSpan.FromMinutes(1) se quiser testar rápido na sua máquina!
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}