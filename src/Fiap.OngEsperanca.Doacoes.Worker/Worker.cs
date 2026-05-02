using System.Text;
using System.Text.Json;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Events;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Fiap.OngEsperanca.Doacoes.Worker;

public class Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(" Worker de Doações (com EF Core) iniciado...");

        var factory = new ConnectionFactory { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        const string queueName = "doacoes-recebidas";
        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // 1. Transforma o JSON de volta para o objeto C#
                var opcoesJson = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var evento = JsonSerializer.Deserialize<DoacaoRecebidaEvent>(message, opcoesJson);

                if (evento != null)
                {
                    logger.LogInformation(" Processando doação de R$ {Valor} para a campanha {Id}", evento.Valor, evento.CampanhaId);

                    // 2. Abre um "miniescopo" para ir no banco de dados e voltar
                    using var scope = scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<CampanhasDbContext>();

                    // 3. Busca a campanha correspondente
                    var campanha = await dbContext.Set<Campanha>().FirstOrDefaultAsync(c => c.Id == evento.CampanhaId, stoppingToken);

                    if (campanha != null)
                    {
                        // 4. Aplica a regra de negócio da Entidade e salva no banco!
                        campanha.AdicionarArrecadacao(evento.Valor);
                        await dbContext.SaveChangesAsync(stoppingToken);

                        logger.LogInformation(" Banco atualizado! Novo total da campanha: R$ {Total}", campanha.ValorTotalArrecadado);
                    }
                    else
                    {
                        logger.LogWarning(" Campanha {Id} não encontrada no banco.", evento.CampanhaId);
                    }
                }

                // 5. Apaga a mensagem da fila (ACK)
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                // Se o banco cair ou der pau, o erro é pego aqui e a mensagem VOLTA pra fila automaticamente!
                logger.LogError(ex, " Erro ao salvar doação.");
            }
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}