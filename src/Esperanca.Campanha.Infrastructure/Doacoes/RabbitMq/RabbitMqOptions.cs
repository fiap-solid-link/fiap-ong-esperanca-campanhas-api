namespace Esperanca.Campanha.Infrastructure.Doacoes.RabbitMq;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string User { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";

    public string Exchange { get; init; } = "esperanca.doacoes";
    public string Queue { get; init; } = "doacoes-recebidas";
    public string RoutingKey { get; init; } = "recebida";

    public string DeadLetterExchange { get; init; } = "esperanca.doacoes.dlx";
    public string DeadLetterQueue { get; init; } = "doacoes-recebidas-dlq";
}
