using Fiap.OngEsperanca.Campanhas.Api.Features.Doacoes.AtualizarArrecadacao;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Mensageria.Consumers;

public record DoacaoProcessadaEvent(Guid CampanhaId, Guid DoadorId, decimal Valor, DateTime DataProcessamento, bool Sucesso);

public class DoacaoProcessadaConsumer : BackgroundService
{
    private readonly ILogger<DoacaoProcessadaConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    // Injetamos a FÁBRICA agora, e não a conexão direto
    private readonly ConnectionFactory _factory;
    private IConnection _connection;
    private IChannel _channel;

    private const string Fila = "doacoes-processadas";

    public DoacaoProcessadaConsumer(
        ILogger<DoacaoProcessadaConsumer> logger,
        IServiceScopeFactory scopeFactory,
        ConnectionFactory factory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 1. Criando a conexão e o canal de forma 100% assíncrona (Padrão V7)
        _connection = await _factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(queue: Fila, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var mensagem = Encoding.UTF8.GetString(body);

            try
            {
                var evento = JsonSerializer.Deserialize<DoacaoProcessadaEvent>(mensagem, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (evento != null && evento.Sucesso)
                {
                    _logger.LogInformation($"[RabbitMQ] Mensagem recebida: Doação de R$ {evento.Valor} para Campanha {evento.CampanhaId}");

                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var command = new AtualizarArrecadacaoCommand(evento.CampanhaId, evento.Valor);
                    await mediator.Send(command, stoppingToken);

                    _logger.LogInformation("[RabbitMQ] Arrecadação atualizada no banco com sucesso!");
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RabbitMQ] Erro ao processar atualização de arrecadação.");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true, cancellationToken: stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(queue: Fila, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

        // Mantém a thread viva esperando as mensagens caírem na fila
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Fechamento limpo para não deixar conexões fantasmas (zumbis) no servidor
        if (_channel != null) await _channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}