using Esperanca.Campanha.Application.Doacoes._Shared;
using Esperanca.Campanha.Application.Doacoes._Shared.Contracts;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes.EnviarIntencao.Mocks;

public class DoacaoPublisherMock
{
    public IDoacaoPublisher Instance { get; }

    public DoacaoPublisherMock()
    {
        Instance = Substitute.For<IDoacaoPublisher>();
        Instance.PublicarRecebidaAsync(Arg.Any<DoacaoRecebidaEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
    }

    public DoacaoPublisherMock SetupPublicarRecebidaThrows(Exception exception)
    {
        Instance.PublicarRecebidaAsync(Arg.Any<DoacaoRecebidaEvent>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(exception);
        return this;
    }

    public void VerifyPublicarRecebidaCalled() =>
        Instance.Received(1).PublicarRecebidaAsync(Arg.Any<DoacaoRecebidaEvent>(), Arg.Any<CancellationToken>());

    public void VerifyPublicarRecebidaNotCalled() =>
        Instance.DidNotReceive().PublicarRecebidaAsync(Arg.Any<DoacaoRecebidaEvent>(), Arg.Any<CancellationToken>());

    public void VerifyPublicarRecebidaCalledWith(Func<DoacaoRecebidaEvent, bool> predicate) =>
        Instance.Received(1).PublicarRecebidaAsync(
            Arg.Is<DoacaoRecebidaEvent>(e => predicate(e)),
            Arg.Any<CancellationToken>());
}
