using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.ListarCampanhas;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fiap.OngEsperanca.Campanhas.Api.Tests.Features;

public class ListarCampanhasHandlerTests
{
    private readonly CampanhasDbContext _dbContextFake;
    private readonly ListarCampanhasHandler _handler;

    public ListarCampanhasHandlerTests()
    {
        // 1. Criamos o nosso banco de dados em memória limpinho
        var options = new DbContextOptionsBuilder<CampanhasDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContextFake = new CampanhasDbContext(options);

        // 2. Injetamos apenas o contexto (ele não precisa de repositório)
        _handler = new ListarCampanhasHandler(_dbContextFake);
    }

    [Fact(DisplayName = "Deve retornar lista vazia e Status 200 quando não houver campanhas no banco")]
    public async Task Handle_SemCampanhas_DeveRetornarListaVazia()
    {
        // Arrange
        var query = new ListarCampanhasQuery();

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.StatusCode.Should().Be(200);
        resultado.Sucesso.Should().BeTrue();

        // Verifica se a lista retornada no "Dados" está vazia
        resultado.Dados.Should().BeEmpty();
    }

    [Fact(DisplayName = "Deve retornar as campanhas mapeadas para Response e Status 200")]
    public async Task Handle_ComCampanhas_DeveRetornarCampanhasMapeadas()
    {
        // Arrange
        var query = new ListarCampanhasQuery();

        // Criamos as entidades puras de domínio
        var campanha1 = new Campanha("Agasalho 2026", "Doações", DateTime.UtcNow, DateTime.UtcNow.AddDays(10), 1000m);
        var campanha2 = new Campanha("Alimentos", "Cestas", DateTime.UtcNow, DateTime.UtcNow.AddDays(20), 2000m);

        // Salvamos no banco em memória ANTES do Handler rodar
        _dbContextFake.Set<Campanha>().AddRange(campanha1, campanha2);
        await _dbContextFake.SaveChangesAsync();

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.StatusCode.Should().Be(200);
        resultado.Sucesso.Should().BeTrue();

        // Transformamos o IEnumerable em uma lista para facilitar os testes
        var listaDeRetorno = resultado.Dados.ToList();

        // Devemos ter exatamente 2 itens
        listaDeRetorno.Should().HaveCount(2);

        // Verifica se o Entity Framework fez o mapeamento (Select) corretamente para o "CampanhaResponse"
        listaDeRetorno.Should().Contain(c => c.Titulo == "Agasalho 2026" && c.MetaFinanceira == 1000m);
        listaDeRetorno.Should().Contain(c => c.Titulo == "Alimentos" && c.MetaFinanceira == 2000m);
    }
}