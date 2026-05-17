using Esperanca.Campanha.Application.Transparencia._Shared;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

public class TransparenciaProjectionWriterMock
{
    public ITransparenciaProjectionWriter Instance { get; }

    public TransparenciaProjectionWriterMock()
    {
        Instance = Substitute.For<ITransparenciaProjectionWriter>();

        Instance.CriarProjecaoCampanhaAsync(
                Arg.Any<CriarCampanhaProjectionInput>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        Instance.AtualizarStatusCampanhaAsync(
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
    }

    public void VerifyCriarProjecaoCampanhaCalled() =>
        Instance.Received(1).CriarProjecaoCampanhaAsync(
            Arg.Any<CriarCampanhaProjectionInput>(),
            Arg.Any<CancellationToken>());

    public void VerifyCriarProjecaoCampanhaNotCalled() =>
        Instance.DidNotReceive().CriarProjecaoCampanhaAsync(
            Arg.Any<CriarCampanhaProjectionInput>(),
            Arg.Any<CancellationToken>());

    public void VerifyAtualizarStatusCampanhaCalled() =>
        Instance.Received(1).AtualizarStatusCampanhaAsync(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<CancellationToken>());

    public void VerifyAtualizarStatusCampanhaNotCalled() =>
        Instance.DidNotReceive().AtualizarStatusCampanhaAsync(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<CancellationToken>());
}
