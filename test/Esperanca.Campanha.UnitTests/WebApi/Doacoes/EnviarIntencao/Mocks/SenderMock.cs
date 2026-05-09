using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Doacoes.EnviarIntencao;
using MediatR;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.WebApi.Doacoes.EnviarIntencao.Mocks;

public class SenderMock
{
    public ISender Instance { get; }

    public SenderMock()
    {
        Instance = Substitute.For<ISender>();
    }

    public SenderMock SetupEnviarIntencaoSuccess(IntencaoDoacaoDto dto)
    {
        Instance.Send(Arg.Any<EnviarIntencaoDoacaoCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<IntencaoDoacaoDto>.Accepted(dto));
        return this;
    }

    public SenderMock SetupEnviarIntencaoNotFound(string erro)
    {
        Instance.Send(Arg.Any<EnviarIntencaoDoacaoCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<IntencaoDoacaoDto>.NotFound(erro));
        return this;
    }

    public SenderMock SetupEnviarIntencaoFail(string erro)
    {
        Instance.Send(Arg.Any<EnviarIntencaoDoacaoCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<IntencaoDoacaoDto>.Fail(erro));
        return this;
    }

    public void VerifyEnviarIntencaoCalled() =>
        Instance.Received(1).Send(Arg.Any<EnviarIntencaoDoacaoCommand>(), Arg.Any<CancellationToken>());
}
