using System.Text.Json;
using Esperanca.Campanha.Application.Doacoes._Shared.Contracts;
using Esperanca.Campanha.Application.Doacoes.ProcessarDoacaoProcessada;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Esperanca.Campanha.Infrastructure.Doacoes.RabbitMq;

public sealed class RabbitMqDoacaoProcessadaConsumer(
    IOptions<RabbitMqOptions> options,
    IServiceScopeFactory scopeFactory,
    ILogger<RabbitMqDoacaoProcessadaConsumer> logger)
    : BackgroundService
{
    private readonly RabbitMqOptions _opts = options.Value;
    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await ConnectAndConsumeAsync(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // shutdown normal
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "RabbitMqDoacaoProcessadaConsumer encerrou com erro. Será reiniciado pelo host se configurado.");
            throw;
        }
    }

    private async Task ConnectAndConsumeAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            HostName    = _opts.Host,
            Port        = _opts.Port,
            UserName    = _opts.User,
            Password    = _opts.Password,
            VirtualHost = _opts.VirtualHost
        };

        _connection = await factory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);

        await RabbitMqTopology.DeclareDeadLetterAsync(_channel, _opts.DeadLetterExchange, _opts.ProcessadaDeadLetterQueue, ct);
        await RabbitMqTopology.DeclareWorkQueueAsync(
            _channel,
            _opts.Exchange,
            _opts.ProcessadaQueue,
            _opts.ProcessadaRoutingKey,
            _opts.DeadLetterExchange,
            ct);

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: _opts.PrefetchCount, global: false, cancellationToken: ct);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _channel.BasicConsumeAsync(
            queue: _opts.ProcessadaQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: ct);

        logger.LogInformation(
            "Consumer ouvindo {Queue} (prefetch={Prefetch}).",
            _opts.ProcessadaQueue, _opts.PrefetchCount);

        await Task.Delay(Timeout.Infinite, ct);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        if (_channel is null) return;

        var deliveryTag = ea.DeliveryTag;

        try
        {
            var evento = JsonSerializer.Deserialize<DoacaoProcessadaEvent>(ea.Body.Span)
                         ?? throw new InvalidOperationException("DoacaoProcessadaEvent payload vazio.");

            using var scope = scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new ProcessarDoacaoProcessadaCommand(
                evento.IdDoacao,
                evento.IdCampanha,
                evento.Valor,
                evento.DataProcessamento));

            await _channel.BasicAckAsync(deliveryTag, multiple: false);
        }
        catch (JsonException jsonEx)
        {
            logger.LogError(jsonEx,
                "Mensagem com payload inválido — descartando para DLQ (deliveryTag={DeliveryTag}).", deliveryTag);
            await _channel.BasicNackAsync(deliveryTag, multiple: false, requeue: false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Falha ao processar DoacaoProcessadaEvent — encaminhando para DLQ (deliveryTag={DeliveryTag}).", deliveryTag);
            await _channel.BasicNackAsync(deliveryTag, multiple: false, requeue: false);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        if (_channel is not null) await _channel.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
    }
}
