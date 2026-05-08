using Esperanca.Campanha.Application.Doacoes._Shared.Contracts;

namespace Esperanca.Campanha.Application.Doacoes._Shared;

public interface IDoacaoPublisher
{
    Task PublicarRecebidaAsync(DoacaoRecebidaEvent evento, CancellationToken cancellationToken = default);
}
