using System.Text.Json;
using Esperanca.Campanha.Application.Doacoes._Shared;
using Esperanca.Campanha.Application.Doacoes._Shared.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Esperanca.Campanha.Infrastructure.Doacoes.RabbitMq;

public sealed class RabbitMqDoacaoPublisher(
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqDoacaoPublisher> logger)
    : IDoacaoPublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _opts = options.Value;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private IConnection? _connection;
    private IChannel? _channel;

    public async Task PublicarRecebidaAsync(DoacaoRecebidaEvent evento, CancellationToken ct = default)
    {
        var channel = await EnsureChannelAsync(ct);

        var body = JsonSerializer.SerializeToUtf8Bytes(evento);

        var props = new BasicProperties
        {
            ContentType  = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            MessageId    = evento.IdDoacao.ToString(),
            CorrelationId = evento.IdempotencyKey.ToString(),
            Timestamp    = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await channel.BasicPublishAsync(
            exchange: _opts.Exchange,
            routingKey: _opts.RoutingKey,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: ct);

        logger.LogInformation(
            "DoacaoRecebidaEvent publicado em {Exchange}/{RoutingKey} (idDoacao={IdDoacao})",
            _opts.Exchange, _opts.RoutingKey, evento.IdDoacao);
    }

    private async Task<IChannel> EnsureChannelAsync(CancellationToken ct)
    {
        if (_channel is { IsOpen: true })
            return _channel;

        await _gate.WaitAsync(ct);
        try
        {
            if (_channel is { IsOpen: true })
                return _channel;

            _connection ??= await CreateConnectionAsync(ct);
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
            await DeclareTopologyAsync(_channel, ct);
            return _channel;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<IConnection> CreateConnectionAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            HostName    = _opts.Host,
            Port        = _opts.Port,
            UserName    = _opts.User,
            Password    = _opts.Password,
            VirtualHost = _opts.VirtualHost
        };

        logger.LogInformation("Conectando ao RabbitMQ em {Host}:{Port}", _opts.Host, _opts.Port);
        return await factory.CreateConnectionAsync(ct);
    }

    private async Task DeclareTopologyAsync(IChannel channel, CancellationToken ct)
    {
        // Dead letter side
        await channel.ExchangeDeclareAsync(
            exchange: _opts.DeadLetterExchange,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: ct);

        await channel.QueueDeclareAsync(
            queue: _opts.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);

        await channel.QueueBindAsync(
            queue: _opts.DeadLetterQueue,
            exchange: _opts.DeadLetterExchange,
            routingKey: string.Empty,
            cancellationToken: ct);

        // Main exchange + queue with DLX
        await channel.ExchangeDeclareAsync(
            exchange: _opts.Exchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: ct);

        var queueArgs = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = _opts.DeadLetterExchange
        };

        await channel.QueueDeclareAsync(
            queue: _opts.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArgs,
            cancellationToken: ct);

        await channel.QueueBindAsync(
            queue: _opts.Queue,
            exchange: _opts.Exchange,
            routingKey: _opts.RoutingKey,
            cancellationToken: ct);

        logger.LogInformation(
            "Topologia RabbitMQ declarada: exchange={Exchange}, queue={Queue}, dlx={Dlx}, dlq={Dlq}",
            _opts.Exchange, _opts.Queue, _opts.DeadLetterExchange, _opts.DeadLetterQueue);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();
        if (_connection is not null)
            await _connection.DisposeAsync();
        _gate.Dispose();
    }
}
