using Esperanca.Message.Events;

namespace Esperanca.Campanha.Application.Doacoes._Shared;

public interface IDoacaoPublisher
{
    Task PublicarRecebidaAsync(DoacaoRecebida evento, CancellationToken cancellationToken = default);
}
