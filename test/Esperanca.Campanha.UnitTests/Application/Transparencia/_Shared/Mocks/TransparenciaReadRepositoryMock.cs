using Esperanca.Campanha.Application.Transparencia._Shared;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application.Transparencia._Shared.Mocks;

public class TransparenciaReadRepositoryMock
{
    public ITransparenciaReadRepository Instance { get; }

    public TransparenciaReadRepositoryMock()
    {
        Instance = Substitute.For<ITransparenciaReadRepository>();
        Instance.ListarCampanhasAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<CampanhaTransparenciaDto>());
    }

    public TransparenciaReadRepositoryMock SetupPainelMacro(PainelMacroDto? painel)
    {
        Instance.ObterPainelMacroAsync(Arg.Any<CancellationToken>()).Returns(painel);
        return this;
    }

    public TransparenciaReadRepositoryMock SetupListaCampanhas(IReadOnlyList<CampanhaTransparenciaDto> campanhas)
    {
        Instance.ListarCampanhasAsync(Arg.Any<CancellationToken>()).Returns(campanhas);
        return this;
    }

    public TransparenciaReadRepositoryMock SetupDetalheCampanha(Guid idCampanha, CampanhaDetalheDto? detalhe)
    {
        Instance.ObterDetalheCampanhaAsync(idCampanha, Arg.Any<CancellationToken>()).Returns(detalhe);
        return this;
    }
}
