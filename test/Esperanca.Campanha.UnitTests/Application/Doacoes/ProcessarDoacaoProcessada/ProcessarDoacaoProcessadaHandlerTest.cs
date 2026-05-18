using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application.Doacoes.ProcessarDoacaoProcessada.Fakers;
using Esperanca.Campanha.UnitTests.Application.Doacoes.ProcessarDoacaoProcessada.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes.ProcessarDoacaoProcessada;

public class ProcessarDoacaoProcessadaHandlerTest
{
    [Fact]
    public async Task Handle_WhenValorTotalAindaNaoAtingiuMeta_ThenNaoConcluiNemPersiste()
    {
        // Arrange
        var fixture = new ProcessarDoacaoProcessadaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento(meta: 1000m);
        fixture.AppDbContextMock
            .SetupCampanhas([campanha]);

        var command = ProcessarDoacaoProcessadaCommandFaker.Valid(
            campanha.Id,
            valor: 200m,
            valorTotalArrecadado: 200m);

        // Act
        await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        campanha.ValorArrecadado.ShouldBe(0m);
        campanha.Status.ShouldBe(StatusCampanha.EmAndamento);
        fixture.AppDbContextMock.VerifyArrecadacaoProcessadaNotAdded();
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
        fixture.TransparenciaProjectionWriterMock.VerifyAtualizarStatusCampanhaNotCalled();
    }

    [Fact]
    public async Task Handle_WhenValorTotalAtingeMeta_ThenConcluiCampanhaEAtualizaProjecao()
    {
        // Arrange
        var fixture = new ProcessarDoacaoProcessadaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento(meta: 500m);
        fixture.AppDbContextMock
            .SetupCampanhas([campanha])
            .SetupSaveChangesSuccess();

        var command = ProcessarDoacaoProcessadaCommandFaker.Valid(
            campanha.Id,
            valor: 300m,
            valorTotalArrecadado: 600m);

        // Act
        await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        campanha.ValorArrecadado.ShouldBe(0m);
        campanha.Status.ShouldBe(StatusCampanha.Concluida);
        fixture.AppDbContextMock.VerifyArrecadacaoProcessadaNotAdded();
        fixture.AppDbContextMock.VerifySaveChangesCalled();
        fixture.TransparenciaProjectionWriterMock.VerifyAtualizarStatusCampanhaCalled();
    }

    [Fact]
    public async Task Handle_WhenModoSomentePorDataEMetaAtingida_ThenNaoConcluiNemPersiste()
    {
        // Arrange
        var fixture = new ProcessarDoacaoProcessadaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamentoSomentePorData(meta: 500m);
        fixture.AppDbContextMock
            .SetupCampanhas([campanha]);

        var command = ProcessarDoacaoProcessadaCommandFaker.Valid(
            campanha.Id,
            valor: 1000m,
            valorTotalArrecadado: 1000m);

        // Act
        await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        campanha.ValorArrecadado.ShouldBe(0m);
        campanha.Status.ShouldBe(StatusCampanha.EmAndamento);
        fixture.AppDbContextMock.VerifyArrecadacaoProcessadaNotAdded();
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
        fixture.TransparenciaProjectionWriterMock.VerifyAtualizarStatusCampanhaNotCalled();
    }

    [Fact]
    public async Task Handle_WhenCampanhaNaoExiste_ThenIgnoraSemPersistir()
    {
        // Arrange
        var fixture = new ProcessarDoacaoProcessadaHandlerFixture();
        fixture.AppDbContextMock
            .SetupCampanhas([]);

        var command = ProcessarDoacaoProcessadaCommandFaker.Valid(
            Guid.NewGuid(),
            valor: 100m,
            valorTotalArrecadado: 100m);

        // Act
        await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        fixture.AppDbContextMock.VerifyArrecadacaoProcessadaNotAdded();
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
        fixture.TransparenciaProjectionWriterMock.VerifyAtualizarStatusCampanhaNotCalled();
    }
}
