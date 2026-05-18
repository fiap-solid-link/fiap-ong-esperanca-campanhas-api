using Esperanca.Campanha.Application.Transparencia._Shared;
using Esperanca.Campanha.Infrastructure.Transparencia.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Infrastructure.Transparencia.Mongo;

public sealed class TransparenciaMongoRepositoryTest : IAsyncLifetime
{
    private readonly MongoClient _mongoClient = new("mongodb://localhost:27017");
    private readonly string _databaseName = $"transparencia_tests_{Guid.NewGuid():N}";
    private readonly TransparenciaMongoOptions _options;

    public TransparenciaMongoRepositoryTest()
    {
        _options = new TransparenciaMongoOptions
        {
            DatabaseName = _databaseName,
            PainelMacroCollection = "painel_macro",
            ListaCampanhasCollection = "lista_campanhas",
            CampanhaDetalheCollection = "campanha_detalhe"
        };
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _mongoClient.DropDatabaseAsync(_databaseName);
    }

    [Fact]
    public async Task ObterPainelMacroAsync_DeveRetornarNull_QuandoNaoExistirDocumento()
    {
        var repository = CriarRepository();

        var resultado = await repository.ObterPainelMacroAsync();

        resultado.ShouldBeNull();
    }

    [Fact]
    public async Task ObterPainelMacroAsync_DeveRetornarPainelMaisRecente()
    {
        var database = ObterDatabase();

        await database.GetCollection<BsonDocument>(_options.PainelMacroCollection)
            .InsertManyAsync([
                CriarPainelMacroDocument(
                    totalArrecadado: 100m,
                    totalDoacoes: 1,
                    totalCampanhasAtivas: 2,
                    totalCampanhasConcluidas: 3,
                    atualizadoEm: new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc)),
                CriarPainelMacroDocument(
                    totalArrecadado: 250m,
                    totalDoacoes: 5,
                    totalCampanhasAtivas: 4,
                    totalCampanhasConcluidas: 6,
                    atualizadoEm: new DateTime(2026, 1, 2, 10, 0, 0, DateTimeKind.Utc))
            ]);

        var repository = CriarRepository();

        var resultado = await repository.ObterPainelMacroAsync();

        resultado.ShouldNotBeNull();
        resultado.TotalArrecadado.ShouldBe(250m);
        resultado.TotalDoacoes.ShouldBe(5);
        resultado.TotalCampanhasAtivas.ShouldBe(4);
        resultado.TotalCampanhasConcluidas.ShouldBe(6);
        resultado.TopDoadores.Count.ShouldBe(1);
        resultado.TopDoadores[0].Apelido.ShouldBe("Doador teste");
        resultado.TopDoadores[0].TotalDoado.ShouldBe(250m);
        resultado.TopDoadores[0].QuantidadeDoacoes.ShouldBe(5);
    }

    [Fact]
    public async Task ListarCampanhasAsync_DeveRetornarCampanhasOrdenadasEMapeadas()
    {
        var database = ObterDatabase();

        var campanhaConcluida = Guid.NewGuid();
        var campanhaEmAndamento = Guid.NewGuid();

        await database.GetCollection<BsonDocument>(_options.ListaCampanhasCollection)
            .InsertManyAsync([
                CriarCampanhaListaDocument(
                    campanhaConcluida,
                    "Campanha concluída",
                    1000m,
                    1000m,
                    "Concluida",
                    new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 1, 30, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc)),
                CriarCampanhaListaDocument(
                    campanhaEmAndamento,
                    "Campanha em andamento",
                    500m,
                    100m,
                    "EmAndamento",
                    new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc),
                    null)
            ]);

        var repository = CriarRepository();

        var resultado = await repository.ListarCampanhasAsync();

        resultado.Count.ShouldBe(2);
        resultado[0].Id.ShouldBe(campanhaEmAndamento);
        resultado[0].Titulo.ShouldBe("Campanha em andamento");
        resultado[0].MetaFinanceira.ShouldBe(500m);
        resultado[0].ValorArrecadado.ShouldBe(100m);
        resultado[0].Status.ShouldBe("EmAndamento");

        resultado[1].Id.ShouldBe(campanhaConcluida);
        resultado[1].Status.ShouldBe("Concluida");
    }

    [Fact]
    public async Task ObterDetalheCampanhaAsync_DeveRetornarNull_QuandoNaoEncontrarCampanha()
    {
        var repository = CriarRepository();

        var resultado = await repository.ObterDetalheCampanhaAsync(Guid.NewGuid());

        resultado.ShouldBeNull();
    }

    [Fact]
    public async Task ObterDetalheCampanhaAsync_DeveRetornarDetalheMapeado()
    {
        var database = ObterDatabase();

        var idCampanha = Guid.NewGuid();

        await database.GetCollection<BsonDocument>(_options.CampanhaDetalheCollection)
            .InsertOneAsync(CriarCampanhaDetalheDocument(
                idCampanha,
                "Campanha detalhe",
                "Descrição da campanha",
                1000m,
                300m,
                "EmAndamento",
                new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 1, 30, 0, 0, 0, DateTimeKind.Utc),
                null));

        var repository = CriarRepository();

        var resultado = await repository.ObterDetalheCampanhaAsync(idCampanha);

        resultado.ShouldNotBeNull();
        resultado.Id.ShouldBe(idCampanha);
        resultado.Titulo.ShouldBe("Campanha detalhe");
        resultado.Descricao.ShouldBe("Descrição da campanha");
        resultado.MetaFinanceira.ShouldBe(1000m);
        resultado.ValorArrecadado.ShouldBe(300m);
        resultado.Status.ShouldBe("EmAndamento");
        resultado.Doacoes.Count.ShouldBe(1);
        resultado.Doacoes[0].ApelidoDoador.ShouldBe("Doador anônimo");
        resultado.Doacoes[0].Valor.ShouldBe(300m);
    }

    [Fact]
    public void CriarListaDocument_DeveMapearInputParaDocumento()
    {
        var idCampanha = Guid.NewGuid();
        var dataInicio = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dataFim = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);

        var input = new CriarCampanhaProjectionInput(
            idCampanha,
            "Campanha teste",
            "Descrição teste",
            1000m,
            500m,
            "Cadastrada",
            dataInicio,
            dataFim,
            DateTime.UtcNow);

        var document = TransparenciaMongoRepository.CriarListaDocument(input);

        document.IdCampanha.ShouldBe(idCampanha);
        document.Titulo.ShouldBe("Campanha teste");
        document.MetaFinanceira.ShouldBe(1000m);
        document.ValorArrecadado.ShouldBe(0m);
        document.Status.ShouldBe("Cadastrada");
        document.DataInicio.ShouldBe(dataInicio);
        document.DataFim.ShouldBe(dataFim);
        document.DataEncerramento.ShouldBeNull();
    }

    [Fact]
    public void CriarDetalheDocument_DeveMapearInputParaDocumento()
    {
        var idCampanha = Guid.NewGuid();
        var dataInicio = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dataFim = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);

        var input = new CriarCampanhaProjectionInput(
            idCampanha,
            "Campanha teste",
            "Descrição teste",
            1000m,
            500m,
            "Cadastrada",
            dataInicio,
            dataFim,
            DateTime.UtcNow);

        var document = TransparenciaMongoRepository.CriarDetalheDocument(input);

        document.IdCampanha.ShouldBe(idCampanha);
        document.Titulo.ShouldBe("Campanha teste");
        document.Descricao.ShouldBe("Descrição teste");
        document.MetaFinanceira.ShouldBe(1000m);
        document.ValorArrecadado.ShouldBe(0m);
        document.Status.ShouldBe("Cadastrada");
        document.DataInicio.ShouldBe(dataInicio);
        document.DataFim.ShouldBe(dataFim);
        document.DataEncerramento.ShouldBeNull();
        document.Doacoes.ShouldNotBeNull();
        document.Doacoes.Count.ShouldBe(0);
    }

    [Fact]
    public async Task AtualizarStatusCampanhaAsync_DeveAtualizarListaEDetalhe()
    {
        var database = ObterDatabase();

        var idCampanha = Guid.NewGuid();
        var dataInicio = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dataFim = new DateTime(2026, 1, 30, 0, 0, 0, DateTimeKind.Utc);
        var dataEncerramento = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);

        await database.GetCollection<BsonDocument>(_options.ListaCampanhasCollection)
            .InsertOneAsync(CriarCampanhaListaDocument(
                idCampanha,
                "Campanha lista",
                1000m,
                500m,
                "EmAndamento",
                dataInicio,
                dataFim,
                null));

        await database.GetCollection<BsonDocument>(_options.CampanhaDetalheCollection)
            .InsertOneAsync(CriarCampanhaDetalheDocument(
                idCampanha,
                "Campanha detalhe",
                "Descrição",
                1000m,
                500m,
                "EmAndamento",
                dataInicio,
                dataFim,
                null));

        var repository = CriarRepository();

        await repository.AtualizarStatusCampanhaAsync(
            idCampanha,
            "Concluida",
            dataEncerramento);

        var lista = await database.GetCollection<BsonDocument>(_options.ListaCampanhasCollection)
            .Find(x => x["idCampanha"] == new BsonBinaryData(idCampanha, GuidRepresentation.Standard))
            .FirstOrDefaultAsync();

        var detalhe = await database.GetCollection<BsonDocument>(_options.CampanhaDetalheCollection)
            .Find(x => x["idCampanha"] == new BsonBinaryData(idCampanha, GuidRepresentation.Standard))
            .FirstOrDefaultAsync();

        lista["status"].AsString.ShouldBe("Concluida");
        lista["dataEncerramento"].ToUniversalTime().ShouldBe(dataEncerramento);

        detalhe["status"].AsString.ShouldBe("Concluida");
        detalhe["dataEncerramento"].ToUniversalTime().ShouldBe(dataEncerramento);
    }

    private TransparenciaMongoRepository CriarRepository()
    {
        return new TransparenciaMongoRepository(
            _mongoClient,
            Options.Create(_options));
    }

    private IMongoDatabase ObterDatabase()
    {
        return _mongoClient.GetDatabase(_databaseName);
    }

    private static BsonDocument CriarPainelMacroDocument(
        decimal totalArrecadado,
        int totalDoacoes,
        int totalCampanhasAtivas,
        int totalCampanhasConcluidas,
        DateTime atualizadoEm)
    {
        return new BsonDocument
        {
            ["totalArrecadado"] = new BsonDecimal128(totalArrecadado),
            ["totalDoacoes"] = totalDoacoes,
            ["totalCampanhasAtivas"] = totalCampanhasAtivas,
            ["totalCampanhasConcluidas"] = totalCampanhasConcluidas,
            ["topDoadores"] = new BsonArray
            {
                new BsonDocument
                {
                    ["apelido"] = "Doador teste",
                    ["totalDoado"] = new BsonDecimal128(totalArrecadado),
                    ["quantidadeDoacoes"] = totalDoacoes
                }
            },
            ["atualizadoEm"] = atualizadoEm
        };
    }

    private static BsonDocument CriarCampanhaListaDocument(
        Guid idCampanha,
        string titulo,
        decimal metaFinanceira,
        decimal valorArrecadado,
        string status,
        DateTime dataInicio,
        DateTime dataFim,
        DateTime? dataEncerramento)
    {
        return new BsonDocument
        {
            ["idCampanha"] = new BsonBinaryData(idCampanha, GuidRepresentation.Standard),
            ["titulo"] = titulo,
            ["metaFinanceira"] = new BsonDecimal128(metaFinanceira),
            ["valorArrecadado"] = new BsonDecimal128(valorArrecadado),
            ["status"] = status,
            ["dataInicio"] = dataInicio,
            ["dataFim"] = dataFim,
            ["dataEncerramento"] = dataEncerramento is null ? BsonNull.Value : BsonValue.Create(dataEncerramento.Value)
        };
    }

    private static BsonDocument CriarCampanhaDetalheDocument(
        Guid idCampanha,
        string titulo,
        string descricao,
        decimal metaFinanceira,
        decimal valorArrecadado,
        string status,
        DateTime dataInicio,
        DateTime dataFim,
        DateTime? dataEncerramento)
    {
        return new BsonDocument
        {
            ["idCampanha"] = new BsonBinaryData(idCampanha, GuidRepresentation.Standard),
            ["titulo"] = titulo,
            ["descricao"] = descricao,
            ["metaFinanceira"] = new BsonDecimal128(metaFinanceira),
            ["valorArrecadado"] = new BsonDecimal128(valorArrecadado),
            ["status"] = status,
            ["dataInicio"] = dataInicio,
            ["dataFim"] = dataFim,
            ["dataEncerramento"] = dataEncerramento is null ? BsonNull.Value : BsonValue.Create(dataEncerramento.Value),
            ["doacoes"] = new BsonArray
            {
                new BsonDocument
                {
                    ["apelidoDoador"] = "Doador anônimo",
                    ["valor"] = new BsonDecimal128(valorArrecadado),
                    ["data"] = DateTime.UtcNow
                }
            }
        };
    }
}