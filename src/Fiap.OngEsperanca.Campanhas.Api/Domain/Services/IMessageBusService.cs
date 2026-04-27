using System.Threading.Tasks;

namespace Fiap.OngEsperanca.Campanhas.Api.Domain.Services;

public interface IMessageBusService
{
    // Método genérico para publicar qualquer evento em qualquer fila
    Task PublicarAsync<T>(T mensagem, string nomeFila) where T : class;
}