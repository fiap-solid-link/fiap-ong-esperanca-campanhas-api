using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Services;
using RabbitMQ.Client;

namespace Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Mensageria;

public class RabbitMqService : IMessageBusService
{
    // Adicionamos o "async" na assinatura do método
    public async Task PublicarAsync<T>(T mensagem, string nomeFila) where T : class
    {
        // Conecta no RabbitMQ local
        var factory = new ConnectionFactory { HostName = "localhost" };

        // Agora usamos 'await' e os métodos terminados em 'Async'
        await using var connection = await factory.CreateConnectionAsync();

        // CreateModel foi renomeado para CreateChannelAsync
        await using var channel = await connection.CreateChannelAsync();

        // Garante que a fila existe lá no servidor
        await channel.QueueDeclareAsync(queue: nomeFila,
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

        // Transforma o objeto do evento em JSON
        var json = JsonSerializer.Serialize(mensagem);
        var body = Encoding.UTF8.GetBytes(json);

        // Publica a mensagem na fila
        await channel.BasicPublishAsync(exchange: string.Empty,
                                        routingKey: nomeFila,
                                        body: body);
    }
}