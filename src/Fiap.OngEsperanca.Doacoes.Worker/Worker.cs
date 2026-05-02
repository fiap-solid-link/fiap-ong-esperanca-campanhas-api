using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Fiap.OngEsperanca.Doacoes.Worker;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("🎧 Iniciando o Worker de Doações. Escutando a fila...");

        // 1. Conecta no RabbitMQ
        var factory = new ConnectionFactory { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        const string queueName = "doacoes-recebidas";

        // 2. Garante que a fila existe (mesma configuração da API)
        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

        // 3. Cria o "Escutador" (Consumidor)
        var consumer = new AsyncEventingBasicConsumer(channel);

        // 4. O que fazer quando uma mensagem chegar?
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            logger.LogInformation("💸 [NOVA DOAÇÃO] Recebida da fila com sucesso: {Message}", message);

            // TODO: No futuro, aqui vai a lógica de ir no banco e somar o valor na campanha

            // 5. Avisa o RabbitMQ que processamos com sucesso e ele pode apagar da fila (ACK)
            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
        };

        // 6. Liga o "Escutador" na fila
        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

        // 7. Mantém o Worker rodando infinitamente
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}