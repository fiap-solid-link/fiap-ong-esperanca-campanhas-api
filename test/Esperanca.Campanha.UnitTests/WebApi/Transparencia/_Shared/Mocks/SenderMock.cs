using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Transparencia._Shared;
using Esperanca.Campanha.Application.Transparencia.ConsultarDetalheCampanha;
using Esperanca.Campanha.Application.Transparencia.ConsultarListaCampanhas;
using Esperanca.Campanha.Application.Transparencia.ConsultarPainelMacro;
using MediatR;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.WebApi.Transparencia._Shared.Mocks;

public class SenderMock
{
    public ISender Instance { get; }

    public SenderMock() => Instance = Substitute.For<ISender>();

    // ── ObterPainel ──────────────────────────────────────────────────────────
    public SenderMock SetupObterPainelSuccess(PainelMacroDto dto)
    {
        Instance.Send(Arg.Any<ConsultarPainelMacroQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<PainelMacroDto>.Ok(dto));
        return this;
    }

    public void VerifyObterPainelCalled() =>
        Instance.Received(1).Send(Arg.Any<ConsultarPainelMacroQuery>(), Arg.Any<CancellationToken>());

    // ── ListarCampanhas ──────────────────────────────────────────────────────
    public SenderMock SetupListarCampanhasSuccess(IReadOnlyList<CampanhaTransparenciaDto> lista)
    {
        Instance.Send(Arg.Any<ConsultarListaCampanhasQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<CampanhaTransparenciaDto>>.Ok(lista));
        return this;
    }

    public void VerifyListarCampanhasCalled() =>
        Instance.Received(1).Send(Arg.Any<ConsultarListaCampanhasQuery>(), Arg.Any<CancellationToken>());

    // ── ObterDetalheCampanha ─────────────────────────────────────────────────
    public SenderMock SetupObterDetalheSuccess(CampanhaDetalheDto dto)
    {
        Instance.Send(Arg.Any<ConsultarDetalheCampanhaQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDetalheDto>.Ok(dto));
        return this;
    }

    public SenderMock SetupObterDetalheNotFound(string erro)
    {
        Instance.Send(Arg.Any<ConsultarDetalheCampanhaQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDetalheDto>.NotFound(erro));
        return this;
    }

    public void VerifyObterDetalheCalled() =>
        Instance.Received(1).Send(Arg.Any<ConsultarDetalheCampanhaQuery>(), Arg.Any<CancellationToken>());
}
