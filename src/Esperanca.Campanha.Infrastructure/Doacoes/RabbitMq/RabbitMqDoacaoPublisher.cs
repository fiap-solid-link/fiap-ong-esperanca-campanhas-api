using System.Text;
using System.Text.Json;
using Esperanca.Campanha.Application.Doacoes._Shared;
using Esperanca.Message.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Esperanca.Campanha.Infrastructure.Doacoes.RabbitMq;

public sealed class RabbitMqDoacaoPublisher(
    IOptions<RabbitMqOptions> options,
    IHttpContextAccessor httpContextAccessor,
    ILogger<RabbitMqDoacaoPublisher> logger)
    : IDoacaoPublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _opts = options.Value;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private IConnection? _connection;
    private IChannel? _channel;

    public async Task PublicarRecebidaAsync(DoacaoRecebida evento, CancellationToken ct = default)
    {
        var channel = await EnsureChannelAsync(ct);

        var body = JsonSerializer.SerializeToUtf8Bytes(evento);

        var correlationId = httpContextAccessor.HttpContext?.Items["X-Correlation-Id"]?.ToString()
                            ?? Guid.NewGuid().ToString("N");

        var props = new BasicProperties
        {
            ContentType   = "application/json",
            DeliveryMode  = DeliveryModes.Persistent,
            MessageId     = evento.IdDoacao.ToString(),
            CorrelationId = evento.IdempotencyKey.ToString(),
            Timestamp     = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            Headers       = new Dictionary<string, object?> { ["X-Correlation-Id"] = Encoding.UTF8.GetBytes(correlationId) }
        };

        await channel.BasicPublishAsync(
            exchange: _opts.Exchange,
            routingKey: _opts.RecebidaRoutingKey,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: ct);

        logger.LogInformation(
            "DoacaoRecebida publicada em {Exchange}/{RoutingKey} (idDoacao={IdDoacao})",
            _opts.Exchange, _opts.RecebidaRoutingKey, evento.IdDoacao);
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
        await RabbitMqTopology.DeclareDeadLetterAsync(channel, _opts.DeadLetterExchange, _opts.RecebidaDeadLetterQueue, ct);
        await RabbitMqTopology.DeclareWorkQueueAsync(
            channel,
            _opts.Exchange,
            _opts.RecebidaQueue,
            _opts.RecebidaRoutingKey,
            _opts.DeadLetterExchange,
            ct);

        logger.LogInformation(
            "Topologia RabbitMQ (publisher) declarada: exchange={Exchange}, queue={Queue}, dlx={Dlx}, dlq={Dlq}",
            _opts.Exchange, _opts.RecebidaQueue, _opts.DeadLetterExchange, _opts.RecebidaDeadLetterQueue);
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
