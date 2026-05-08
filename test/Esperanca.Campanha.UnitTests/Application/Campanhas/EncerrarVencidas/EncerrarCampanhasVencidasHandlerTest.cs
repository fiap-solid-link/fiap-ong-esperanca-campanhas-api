using Esperanca.Campanha.Application.Campanhas.EncerrarVencidas;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using Esperanca.Campanha.UnitTests.Application.Campanhas.EncerrarVencidas.Fakers;
using Esperanca.Campanha.UnitTests.Application.Campanhas.EncerrarVencidas.Fixtures;
using Shouldly;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.EncerrarVencidas;

public class EncerrarCampanhasVencidasHandlerTest
{
    private static readonly DateTime Agora = DateTimeProviderMock.DefaultNow;

    [Fact]
    public async Task Handle_WhenCampanhaPorDataVencida_ThenConcluiESalva()
    {
        // Arrange
        var fixture = new EncerrarCampanhasVencidasHandlerFixture();
        var vencida = CampanhaFaker.EmAndamento(dataFim: Agora.AddDays(-1), modo: ModoEncerramento.PorData);
        fixture.AppDbContextMock
            .SetupCampanhas([vencida])
            .SetupSaveChangesSuccess();

        // Act
        await fixture.Handler.Handle(new EncerrarCampanhasVencidasCommand(3), CancellationToken.None);

        // Assert
        vencida.Status.ShouldBe(StatusCampanha.Concluida);
        fixture.AppDbContextMock.VerifySaveChangesCalled();
    }

    [Fact]
    public async Task Handle_WhenCampanhaPorMetaVencidaSemAtingirMeta_ThenNaoConclui()
    {
        // Arrange
        var fixture = new EncerrarCampanhasVencidasHandlerFixture();
        var vencidaPorMeta = CampanhaFaker.EmAndamento(dataFim: Agora.AddDays(-1), modo: ModoEncerramento.PorMeta);
        fixture.AppDbContextMock
            .SetupCampanhas([vencidaPorMeta]);

        // Act
        await fixture.Handler.Handle(new EncerrarCampanhasVencidasCommand(3), CancellationToken.None);

        // Assert
        vencidaPorMeta.Status.ShouldBe(StatusCampanha.EmAndamento);
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }

    [Fact]
    public async Task Handle_WhenCampanhaProximaDoVencimento_ThenNaoConcluiPersistirAlteracao()
    {
        // Arrange
        var fixture = new EncerrarCampanhasVencidasHandlerFixture();
        var proxima = CampanhaFaker.EmAndamento(dataFim: Agora.AddDays(2));
        fixture.AppDbContextMock
            .SetupCampanhas([proxima]);

        // Act
        await fixture.Handler.Handle(new EncerrarCampanhasVencidasCommand(3), CancellationToken.None);

        // Assert
        proxima.Status.ShouldBe(StatusCampanha.EmAndamento);
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }

    [Fact]
    public async Task Handle_WhenCampanhaForaDaJanela_ThenNaoConcluiNemAlertaPersistencia()
    {
        // Arrange
        var fixture = new EncerrarCampanhasVencidasHandlerFixture();
        var distante = CampanhaFaker.EmAndamento(dataFim: Agora.AddDays(20));
        fixture.AppDbContextMock
            .SetupCampanhas([distante]);

        // Act
        await fixture.Handler.Handle(new EncerrarCampanhasVencidasCommand(3), CancellationToken.None);

        // Assert
        distante.Status.ShouldBe(StatusCampanha.EmAndamento);
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }

    [Fact]
    public async Task Handle_WhenSemCampanhasEmAndamento_ThenNaoPersiste()
    {
        // Arrange
        var fixture = new EncerrarCampanhasVencidasHandlerFixture();
        var cadastrada = CampanhaFaker.Cadastrada(dataFim: Agora.AddDays(10));
        fixture.AppDbContextMock
            .SetupCampanhas([cadastrada]);

        // Act
        await fixture.Handler.Handle(new EncerrarCampanhasVencidasCommand(3), CancellationToken.None);

        // Assert
        cadastrada.Status.ShouldBe(StatusCampanha.Cadastrada);
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }

    [Fact]
    public async Task Handle_WhenMisturaDeCenarios_ThenConcluiApenasAsAplicaveis()
    {
        // Arrange
        var fixture = new EncerrarCampanhasVencidasHandlerFixture();
        var vencidaPorData     = CampanhaFaker.EmAndamento(dataFim: Agora.AddDays(-1), modo: ModoEncerramento.PorData);
        var vencidaPorMeta     = CampanhaFaker.EmAndamento(dataFim: Agora.AddDays(-1), modo: ModoEncerramento.PorMeta);
        var proximaDoVenc      = CampanhaFaker.EmAndamento(dataFim: Agora.AddDays(2));
        var distante           = CampanhaFaker.EmAndamento(dataFim: Agora.AddDays(20));
        var campanhas = new List<CampanhaAgg> { vencidaPorData, vencidaPorMeta, proximaDoVenc, distante };
        fixture.AppDbContextMock
            .SetupCampanhas(campanhas)
            .SetupSaveChangesSuccess();

        // Act
        await fixture.Handler.Handle(new EncerrarCampanhasVencidasCommand(3), CancellationToken.None);

        // Assert
        vencidaPorData.Status.ShouldBe(StatusCampanha.Concluida);
        vencidaPorMeta.Status.ShouldBe(StatusCampanha.EmAndamento);
        proximaDoVenc.Status.ShouldBe(StatusCampanha.EmAndamento);
        distante.Status.ShouldBe(StatusCampanha.EmAndamento);
        fixture.AppDbContextMock.VerifySaveChangesCalled();
    }
}
