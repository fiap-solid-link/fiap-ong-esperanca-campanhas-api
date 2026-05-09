using Esperanca.Campanha.Application.Campanhas.Listar;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Listar.Fakers;
using Esperanca.Campanha.UnitTests.Application.Campanhas.Listar.Fixtures;
using Shouldly;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Listar;

public class ListarCampanhasGestorHandlerTest
{
    private static readonly DateTime Agora = DateTimeProviderMock.DefaultNow;

    [Fact]
    public async Task Handle_WhenSemFiltros_ThenRetornaApenasCampanhasDoGestor()
    {
        // Arrange
        var fixture = new ListarCampanhasGestorHandlerFixture();
        var minha1 = CampanhaFaker.Cadastrada("Minha A", Agora);
        var minha2 = CampanhaFaker.Cadastrada("Minha B", Agora.AddDays(1));
        var deOutro = CampanhaFaker.DeOutroGestor("Alheia", Agora);
        fixture.AppDbContextMock.SetupCampanhas([minha1, minha2, deOutro]);

        // Act
        var result = await fixture.Handler.Handle(ListarCampanhasGestorQueryFaker.Default(), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.StatusCode.ShouldBe(200);
        result.Dados.ShouldNotBeNull();
        result.Dados.TotalItens.ShouldBe(2);
        result.Dados.Itens.Select(i => i.Id).ShouldBe([minha2.Id, minha1.Id]);
    }

    [Fact]
    public async Task Handle_WhenListaVazia_ThenRetornaPaginaVaziaSemErro()
    {
        // Arrange
        var fixture = new ListarCampanhasGestorHandlerFixture();
        fixture.AppDbContextMock.SetupCampanhas([]);

        // Act
        var result = await fixture.Handler.Handle(ListarCampanhasGestorQueryFaker.Default(), CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.Dados.ShouldNotBeNull();
        result.Dados.Itens.ShouldBeEmpty();
        result.Dados.TotalItens.ShouldBe(0);
        result.Dados.TotalPaginas.ShouldBe(0);
        result.Dados.Pagina.ShouldBe(1);
        result.Dados.TamanhoPagina.ShouldBe(20);
    }

    [Fact]
    public async Task Handle_WhenFiltroPorStatus_ThenRetornaApenasCampanhasDoStatus()
    {
        // Arrange
        var fixture = new ListarCampanhasGestorHandlerFixture();
        var cadastrada = CampanhaFaker.Cadastrada("Cadastrada", Agora);
        var emAndamento = CampanhaFaker.EmAndamento("Em Andamento", Agora.AddDays(1));
        fixture.AppDbContextMock.SetupCampanhas([cadastrada, emAndamento]);

        // Act
        var result = await fixture.Handler.Handle(
            ListarCampanhasGestorQueryFaker.ComStatus(StatusCampanha.EmAndamento),
            CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.Dados!.TotalItens.ShouldBe(1);
        result.Dados.Itens.Single().Id.ShouldBe(emAndamento.Id);
    }

    [Fact]
    public async Task Handle_WhenFiltroPorIntervalo_ThenRetornaApenasCampanhasNoIntervalo()
    {
        // Arrange
        var fixture = new ListarCampanhasGestorHandlerFixture();
        var antes = CampanhaFaker.Cadastrada("Antes", Agora.AddDays(-30));
        var dentro = CampanhaFaker.Cadastrada("Dentro", Agora);
        var depois = CampanhaFaker.Cadastrada("Depois", Agora.AddDays(30));
        fixture.AppDbContextMock.SetupCampanhas([antes, dentro, depois]);

        // Act
        var result = await fixture.Handler.Handle(
            ListarCampanhasGestorQueryFaker.ComIntervaloDataInicio(diasAtras: 5, diasFrente: 5),
            CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.Dados!.TotalItens.ShouldBe(1);
        result.Dados.Itens.Single().Id.ShouldBe(dentro.Id);
    }

    [Fact]
    public async Task Handle_WhenPaginacao_ThenRetornaApenasItensDaPagina()
    {
        // Arrange
        var fixture = new ListarCampanhasGestorHandlerFixture();
        var campanhas = new List<CampanhaAgg>
        {
            CampanhaFaker.Cadastrada("A", Agora),
            CampanhaFaker.Cadastrada("B", Agora.AddDays(1)),
            CampanhaFaker.Cadastrada("C", Agora.AddDays(2)),
            CampanhaFaker.Cadastrada("D", Agora.AddDays(3)),
            CampanhaFaker.Cadastrada("E", Agora.AddDays(4)),
        };
        fixture.AppDbContextMock.SetupCampanhas(campanhas);

        // Act
        var result = await fixture.Handler.Handle(
            ListarCampanhasGestorQueryFaker.ComPaginacao(pagina: 2, tamanhoPagina: 2),
            CancellationToken.None);

        // Assert
        result.Sucesso.ShouldBeTrue();
        result.Dados!.TotalItens.ShouldBe(5);
        result.Dados.TotalPaginas.ShouldBe(3);
        result.Dados.Pagina.ShouldBe(2);
        result.Dados.TamanhoPagina.ShouldBe(2);
        result.Dados.Itens.Select(i => i.Titulo).ShouldBe(["C", "B"]);
    }
}
