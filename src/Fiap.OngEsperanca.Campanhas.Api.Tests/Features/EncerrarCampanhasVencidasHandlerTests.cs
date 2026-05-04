using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.EncerrarVencidas;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fiap.OngEsperanca.Campanhas.Api.Tests.Features;

public class EncerrarCampanhasVencidasHandlerTests
{
    private readonly CampanhasDbContext _dbContextFake;
    private readonly EncerrarCampanhasVencidasHandler _handler;

    public EncerrarCampanhasVencidasHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampanhasDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContextFake = new CampanhasDbContext(options);

        // Repare que esse Handler não usa Repository, ele usa o DbContext direto para ser mais performático no WHERE!
        _handler = new EncerrarCampanhasVencidasHandler(_dbContextFake);
    }

    [Fact(DisplayName = "Encerrar Vencidas: Deve retornar 0 quando não houver campanhas vencidas")]
    public async Task Handle_SemCampanhasVencidas_DeveRetornarZero()
    {
        // Arrange - Cria uma campanha que AINDA NÃO VENCEU
        var campanhaAtiva = new Campanha("Teste", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(10), 1000m);
        campanhaAtiva.Ativar();

        _dbContextFake.Campanhas.Add(campanhaAtiva);
        await _dbContextFake.SaveChangesAsync();

        var comando = new EncerrarCampanhasVencidasCommand();

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().Be(0); // Zero campanhas alteradas
    }

    [Fact(DisplayName = "Encerrar Vencidas: Deve encerrar apenas as campanhas vencidas que estão Em Andamento")]
    public async Task Handle_ComCampanhasVencidas_DeveEncerrarERetornarQuantidade()
    {
        // Arrange
        // 1. Campanha no prazo (NÃO deve ser encerrada)
        var campanhaNoPrazo = new Campanha("No Prazo", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(10), 1000m);
        campanhaNoPrazo.Ativar();

        // 2. Campanha Cadastrada (NÃO deve ser encerrada, pois a regra diz que tem que estar EmAndamento)
        var campanhaCadastrada = new Campanha("Cadastrada", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(10), 1000m);
        typeof(Campanha).GetProperty("DataFim")!.SetValue(campanhaCadastrada, DateTime.UtcNow.AddDays(-1)); // Truque do tempo!

        // 3. Campanha Vencida e Em Andamento (ESSA DEVE SER ENCERRADA!)
        var campanhaVencida = new Campanha("Vencida", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(10), 1000m);
        campanhaVencida.Ativar();
        // Truque de Reflection para simular o tempo passando e vencendo a campanha no banco
        typeof(Campanha).GetProperty("DataFim")!.SetValue(campanhaVencida, DateTime.UtcNow.AddDays(-1));

        _dbContextFake.Campanhas.AddRange(campanhaNoPrazo, campanhaCadastrada, campanhaVencida);
        await _dbContextFake.SaveChangesAsync();

        var comando = new EncerrarCampanhasVencidasCommand();

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().Be(1); // Apenas 1 campanha devia ser alterada!

        // Verifica o banco de dados para garantir que a transição de Status ocorreu apenas na correta
        var campanhasNoBanco = await _dbContextFake.Campanhas.ToListAsync();

        campanhasNoBanco.First(c => c.Titulo == "No Prazo").Status.Should().Be(StatusCampanha.EmAndamento);
        campanhasNoBanco.First(c => c.Titulo == "Cadastrada").Status.Should().Be(StatusCampanha.Cadastrada);
        campanhasNoBanco.First(c => c.Titulo == "Vencida").Status.Should().Be(StatusCampanha.Concluida);
    }
}