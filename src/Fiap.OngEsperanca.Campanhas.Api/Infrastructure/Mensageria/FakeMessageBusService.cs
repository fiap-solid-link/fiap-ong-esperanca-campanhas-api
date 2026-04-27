using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Mensageria;

// Esse cara vai assinar o contrato IMessageBusService
public class FakeMessageBusService(ILogger<FakeMessageBusService> logger) : IMessageBusService
{
    public Task PublicarAsync<T>(T mensagem, string nomeFila) where T : class
    {
        // Em vez de mandar pro RabbitMQ, ele apenas "grita" no console que deu certo
        logger.LogInformation("[RABBITMQ FAKE] Mensagem do tipo {Tipo} enviada para a fila '{Fila}'", typeof(T).Name, nomeFila);

        return Task.CompletedTask;
    }
}