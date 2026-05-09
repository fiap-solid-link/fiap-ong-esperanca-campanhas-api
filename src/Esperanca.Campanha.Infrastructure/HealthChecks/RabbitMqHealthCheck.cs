using Esperanca.Campanha.Infrastructure.Doacoes.RabbitMq;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Esperanca.Campanha.Infrastructure.HealthChecks;

public sealed class RabbitMqHealthCheck(IOptions<RabbitMqOptions> options) : IHealthCheck
{
    private readonly RabbitMqOptions _opts = options.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName    = _opts.Host,
                Port        = _opts.Port,
                UserName    = _opts.User,
                Password    = _opts.Password,
                VirtualHost = _opts.VirtualHost
            };

            await using var conn = await factory.CreateConnectionAsync(ct);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ indisponível.", ex);
        }
    }
}
