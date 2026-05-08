using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application.Doacoes.ProcessarDoacaoProcessada.Fakers;
using Esperanca.Campanha.UnitTests.Application.Doacoes.ProcessarDoacaoProcessada.Fixtures;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Application.Doacoes.ProcessarDoacaoProcessada;

public class ProcessarDoacaoProcessadaHandlerTest
{
    [Fact]
    public async Task Handle_WhenDoacaoNova_ThenRegistraArrecadacaoEPersisteIdempotencia()
    {
        // Arrange
        var fixture = new ProcessarDoacaoProcessadaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento(meta: 1000m);
        fixture.AppDbContextMock
            .SetupCampanhas([campanha])
            .SetupArrecadacoesProcessadas([])
            .SetupSaveChangesSuccess();

        var command = ProcessarDoacaoProcessadaCommandFaker.Valid(campanha.Id, valor: 200m);

        // Act
        await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        campanha.ValorArrecadado.ShouldBe(200m);
        campanha.Status.ShouldBe(StatusCampanha.EmAndamento);
        fixture.AppDbContextMock.VerifyArrecadacaoProcessadaAdded();
        fixture.AppDbContextMock.VerifySaveChangesCalled();
    }

    [Fact]
    public async Task Handle_WhenDoacaoJaProcessada_ThenNaoSomaNoValorArrecadado()
    {
        // Arrange
        var fixture = new ProcessarDoacaoProcessadaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento(meta: 1000m);
        var idDoacao = Guid.NewGuid();
        fixture.AppDbContextMock
            .SetupCampanhas([campanha])
            .SetupArrecadacoesProcessadas([
                ArrecadacaoProcessadaFaker.Existente(idDoacao, campanha.Id, valor: 200m)
            ]);

        var command = ProcessarDoacaoProcessadaCommandFaker.Valid(campanha.Id, valor: 200m, idDoacao: idDoacao);

        // Act
        await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        campanha.ValorArrecadado.ShouldBe(0m);
        fixture.AppDbContextMock.VerifyArrecadacaoProcessadaNotAdded();
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }

    [Fact]
    public async Task Handle_WhenValorAtingeMeta_ThenConcluiCampanha()
    {
        // Arrange
        var fixture = new ProcessarDoacaoProcessadaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamento(meta: 500m);
        fixture.AppDbContextMock
            .SetupCampanhas([campanha])
            .SetupSaveChangesSuccess();

        var command = ProcessarDoacaoProcessadaCommandFaker.Valid(campanha.Id, valor: 600m);

        // Act
        await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        campanha.ValorArrecadado.ShouldBe(600m);
        campanha.Status.ShouldBe(StatusCampanha.Concluida);
        fixture.AppDbContextMock.VerifyArrecadacaoProcessadaAdded();
        fixture.AppDbContextMock.VerifySaveChangesCalled();
    }

    [Fact]
    public async Task Handle_WhenModoSomentePorDataEMetaAtingida_ThenNaoConclui()
    {
        // Arrange
        var fixture = new ProcessarDoacaoProcessadaHandlerFixture();
        var campanha = CampanhaFaker.EmAndamentoSomentePorData(meta: 500m);
        fixture.AppDbContextMock
            .SetupCampanhas([campanha])
            .SetupSaveChangesSuccess();

        var command = ProcessarDoacaoProcessadaCommandFaker.Valid(campanha.Id, valor: 1000m);

        // Act
        await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        campanha.ValorArrecadado.ShouldBe(1000m);
        campanha.Status.ShouldBe(StatusCampanha.EmAndamento);
        fixture.AppDbContextMock.VerifyArrecadacaoProcessadaAdded();
        fixture.AppDbContextMock.VerifySaveChangesCalled();
    }

    [Fact]
    public async Task Handle_WhenCampanhaNaoExiste_ThenIgnoraSemPersistir()
    {
        // Arrange
        var fixture = new ProcessarDoacaoProcessadaHandlerFixture();
        fixture.AppDbContextMock
            .SetupCampanhas([])
            .SetupArrecadacoesProcessadas([]);

        var command = ProcessarDoacaoProcessadaCommandFaker.Valid(Guid.NewGuid(), valor: 100m);

        // Act
        await fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        fixture.AppDbContextMock.VerifyArrecadacaoProcessadaNotAdded();
        fixture.AppDbContextMock.VerifySaveChangesNotCalled();
    }
}
