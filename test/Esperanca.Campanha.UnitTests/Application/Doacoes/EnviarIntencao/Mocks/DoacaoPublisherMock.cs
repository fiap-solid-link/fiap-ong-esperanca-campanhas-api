using Esperanca.Campanha.Application.Doacoes._Shared;
using Esperanca.Message.Events;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes.EnviarIntencao.Mocks;

public class DoacaoPublisherMock
{
    public IDoacaoPublisher Instance { get; }

    public DoacaoPublisherMock()
    {
        Instance = Substitute.For<IDoacaoPublisher>();
        Instance.PublicarRecebidaAsync(Arg.Any<DoacaoRecebida>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
    }

    public DoacaoPublisherMock SetupPublicarRecebidaThrows(Exception exception)
    {
        Instance.PublicarRecebidaAsync(Arg.Any<DoacaoRecebida>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(exception);
        return this;
    }

    public void VerifyPublicarRecebidaCalled() =>
        Instance.Received(1).PublicarRecebidaAsync(Arg.Any<DoacaoRecebida>(), Arg.Any<CancellationToken>());

    public void VerifyPublicarRecebidaNotCalled() =>
        Instance.DidNotReceive().PublicarRecebidaAsync(Arg.Any<DoacaoRecebida>(), Arg.Any<CancellationToken>());

    public void VerifyPublicarRecebidaCalledWith(Func<DoacaoRecebida, bool> predicate) =>
        Instance.Received(1).PublicarRecebidaAsync(
            Arg.Is<DoacaoRecebida>(e => predicate(e)),
            Arg.Any<CancellationToken>());
}
