using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.Application.Campanhas._Shared;
using Esperanca.Campanha.Application.Campanhas.Ativar;
using Esperanca.Campanha.Application.Campanhas.Cancelar;
using Esperanca.Campanha.Application.Campanhas.Criar;
using Esperanca.Campanha.Application.Campanhas.Editar;
using Esperanca.Campanha.Application.Campanhas.Listar;
using Esperanca.Campanha.Application.Campanhas.Obter;
using Esperanca.Campanha.Application.Campanhas.Prorrogar;
using MediatR;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.WebApi.Campanhas._Shared.Mocks;

public class SenderMock
{
    public ISender Instance { get; }

    public SenderMock() => Instance = Substitute.For<ISender>();

    // ── Criar ────────────────────────────────────────────────────────────────
    public SenderMock SetupCriarSuccess(CampanhaDto dto)
    {
        Instance.Send(Arg.Any<CriarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.Created(dto));
        return this;
    }

    public SenderMock SetupCriarFail(string erro)
    {
        Instance.Send(Arg.Any<CriarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.Fail(erro));
        return this;
    }

    public void VerifyCriarCalled() =>
        Instance.Received(1).Send(Arg.Any<CriarCampanhaCommand>(), Arg.Any<CancellationToken>());

    // ── Listar ───────────────────────────────────────────────────────────────
    public SenderMock SetupListarSuccess(PaginaCampanhasDto pagina)
    {
        Instance.Send(Arg.Any<ListarCampanhasGestorQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<PaginaCampanhasDto>.Ok(pagina));
        return this;
    }

    public SenderMock SetupListarFail(string erro)
    {
        Instance.Send(Arg.Any<ListarCampanhasGestorQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<PaginaCampanhasDto>.Fail(erro));
        return this;
    }

    public void VerifyListarCalled() =>
        Instance.Received(1).Send(Arg.Any<ListarCampanhasGestorQuery>(), Arg.Any<CancellationToken>());

    // ── Obter ────────────────────────────────────────────────────────────────
    public SenderMock SetupObterSuccess(CampanhaDto dto)
    {
        Instance.Send(Arg.Any<ObterCampanhaQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.Ok(dto));
        return this;
    }

    public SenderMock SetupObterNotFound(string erro)
    {
        Instance.Send(Arg.Any<ObterCampanhaQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.NotFound(erro));
        return this;
    }

    public void VerifyObterCalled() =>
        Instance.Received(1).Send(Arg.Any<ObterCampanhaQuery>(), Arg.Any<CancellationToken>());

    // ── Editar ───────────────────────────────────────────────────────────────
    public SenderMock SetupEditarSuccess(CampanhaDto dto)
    {
        Instance.Send(Arg.Any<EditarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.Ok(dto));
        return this;
    }

    public SenderMock SetupEditarFail(string erro)
    {
        Instance.Send(Arg.Any<EditarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.Fail(erro));
        return this;
    }

    public SenderMock SetupEditarNotFound(string erro)
    {
        Instance.Send(Arg.Any<EditarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.NotFound(erro));
        return this;
    }

    public void VerifyEditarCalled() =>
        Instance.Received(1).Send(Arg.Any<EditarCampanhaCommand>(), Arg.Any<CancellationToken>());

    public void VerifyEditarCalledWith(Guid expectedId) =>
        Instance.Received(1).Send(
            Arg.Is<EditarCampanhaCommand>(c => c.Id == expectedId),
            Arg.Any<CancellationToken>());

    // ── Ativar ───────────────────────────────────────────────────────────────
    public SenderMock SetupAtivarSuccess(CampanhaDto dto)
    {
        Instance.Send(Arg.Any<AtivarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.Ok(dto));
        return this;
    }

    public SenderMock SetupAtivarFail(string erro)
    {
        Instance.Send(Arg.Any<AtivarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.Fail(erro));
        return this;
    }

    public SenderMock SetupAtivarNotFound(string erro)
    {
        Instance.Send(Arg.Any<AtivarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.NotFound(erro));
        return this;
    }

    public void VerifyAtivarCalled() =>
        Instance.Received(1).Send(Arg.Any<AtivarCampanhaCommand>(), Arg.Any<CancellationToken>());

    // ── Prorrogar ────────────────────────────────────────────────────────────
    public SenderMock SetupProrrogarSuccess(CampanhaDto dto)
    {
        Instance.Send(Arg.Any<ProrrogarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.Ok(dto));
        return this;
    }

    public SenderMock SetupProrrogarFail(string erro)
    {
        Instance.Send(Arg.Any<ProrrogarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.Fail(erro));
        return this;
    }

    public SenderMock SetupProrrogarNotFound(string erro)
    {
        Instance.Send(Arg.Any<ProrrogarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.NotFound(erro));
        return this;
    }

    public void VerifyProrrogarCalled() =>
        Instance.Received(1).Send(Arg.Any<ProrrogarCampanhaCommand>(), Arg.Any<CancellationToken>());

    public void VerifyProrrogarCalledWith(Guid expectedId, DateTime novaDataFim) =>
        Instance.Received(1).Send(
            Arg.Is<ProrrogarCampanhaCommand>(c => c.Id == expectedId && c.NovaDataFim == novaDataFim),
            Arg.Any<CancellationToken>());

    // ── Cancelar ─────────────────────────────────────────────────────────────
    public SenderMock SetupCancelarSuccess(CampanhaDto dto)
    {
        Instance.Send(Arg.Any<CancelarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.Ok(dto));
        return this;
    }

    public SenderMock SetupCancelarFail(string erro)
    {
        Instance.Send(Arg.Any<CancelarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.Fail(erro));
        return this;
    }

    public SenderMock SetupCancelarNotFound(string erro)
    {
        Instance.Send(Arg.Any<CancelarCampanhaCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CampanhaDto>.NotFound(erro));
        return this;
    }

    public void VerifyCancelarCalled() =>
        Instance.Received(1).Send(Arg.Any<CancelarCampanhaCommand>(), Arg.Any<CancellationToken>());
}
