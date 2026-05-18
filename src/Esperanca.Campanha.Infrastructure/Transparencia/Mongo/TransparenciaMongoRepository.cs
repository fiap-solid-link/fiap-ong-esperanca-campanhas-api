using Esperanca.Campanha.Application.Transparencia._Shared;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Esperanca.Campanha.Infrastructure.Transparencia.Mongo;

public sealed class TransparenciaMongoRepository(
    IMongoClient mongoClient,
    IOptions<TransparenciaMongoOptions> options)
    : ITransparenciaReadRepository, ITransparenciaProjectionWriter
{
    private readonly TransparenciaMongoOptions _opts = options.Value;
    private readonly IMongoDatabase _database = mongoClient.GetDatabase(options.Value.DatabaseName);

    public async Task<PainelMacroDto?> ObterPainelMacroAsync(CancellationToken cancellationToken = default)
    {
        var collection = _database.GetCollection<PainelMacroDocument>(_opts.PainelMacroCollection);

        var doc = await collection
            .Find(FilterDefinition<PainelMacroDocument>.Empty)
            .SortByDescending(d => d.AtualizadoEm)
            .FirstOrDefaultAsync(cancellationToken);

        return MapPainelMacro(doc);
    }

    public async Task<IReadOnlyList<CampanhaTransparenciaDto>> ListarCampanhasAsync(CancellationToken cancellationToken = default)
    {
        var collection = _database.GetCollection<CampanhaListaDocument>(_opts.ListaCampanhasCollection);

        var docs = await collection
            .Find(FilterDefinition<CampanhaListaDocument>.Empty)
            .ToListAsync(cancellationToken);

        return MapListaCampanhas(docs);
    }

    public async Task<CampanhaDetalheDto?> ObterDetalheCampanhaAsync(Guid idCampanha, CancellationToken cancellationToken = default)
    {
        var collection = _database.GetCollection<CampanhaDetalheDocument>(_opts.CampanhaDetalheCollection);

        var doc = await collection
            .Find(d => d.IdCampanha == idCampanha)
            .FirstOrDefaultAsync(cancellationToken);

        return MapDetalheCampanha(doc);
    }

    public async Task CriarProjecaoCampanhaAsync(CriarCampanhaProjectionInput campanha, CancellationToken cancellationToken = default)
    {
        var listaCollection = _database.GetCollection<CampanhaListaDocument>(_opts.ListaCampanhasCollection);
        var detalheCollection = _database.GetCollection<CampanhaDetalheDocument>(_opts.CampanhaDetalheCollection);

        var listaDocument = CriarListaDocument(campanha);
        var detalheDocument = CriarDetalheDocument(campanha);

        await listaCollection.ReplaceOneAsync(
            x => x.IdCampanha == campanha.IdCampanha,
            listaDocument,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);

        await detalheCollection.ReplaceOneAsync(
            x => x.IdCampanha == campanha.IdCampanha,
            detalheDocument,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }

    public async Task AtualizarStatusCampanhaAsync(Guid idCampanha, string status, DateTime? dataEncerramento, CancellationToken cancellationToken = default)
    {
        var listaCollection = _database.GetCollection<CampanhaListaDocument>(_opts.ListaCampanhasCollection);
        var detalheCollection = _database.GetCollection<CampanhaDetalheDocument>(_opts.CampanhaDetalheCollection);

        var updateLista = Builders<CampanhaListaDocument>.Update
            .Set(x => x.Status, status)
            .Set(x => x.DataEncerramento, dataEncerramento);

        var updateDetalhe = Builders<CampanhaDetalheDocument>.Update
            .Set(x => x.Status, status)
            .Set(x => x.DataEncerramento, dataEncerramento);

        await listaCollection.UpdateOneAsync(
            x => x.IdCampanha == idCampanha,
            updateLista,
            cancellationToken: cancellationToken);

        await detalheCollection.UpdateOneAsync(
            x => x.IdCampanha == idCampanha,
            updateDetalhe,
            cancellationToken: cancellationToken);
    }


    internal static PainelMacroDto? MapPainelMacro(PainelMacroDocument? doc)
    {
        if (doc is null) return null;

        return new PainelMacroDto(
            doc.TotalArrecadado,
            doc.TotalDoacoes,
            doc.TotalCampanhasAtivas,
            doc.TotalCampanhasConcluidas,
            doc.TopDoadores
                .Select(t => new TopDoadorDto(t.Apelido, t.TotalDoado, t.QuantidadeDoacoes))
                .ToList(),
            doc.AtualizadoEm);
    }

    internal static IReadOnlyList<CampanhaTransparenciaDto> MapListaCampanhas(IEnumerable<CampanhaListaDocument> docs) =>
        docs
            .OrderBy(d => d.Status == "EmAndamento" ? 0 : 1)
            .ThenByDescending(d => d.DataInicio)
            .Select(d => new CampanhaTransparenciaDto(
                d.IdCampanha,
                d.Titulo,
                d.MetaFinanceira,
                d.ValorArrecadado,
                d.Status,
                d.DataInicio,
                d.DataFim,
                d.DataEncerramento))
            .ToList();

    internal static CampanhaDetalheDto? MapDetalheCampanha(CampanhaDetalheDocument? doc)
    {
        if (doc is null) return null;

        return new CampanhaDetalheDto(
            doc.IdCampanha,
            doc.Titulo,
            doc.Descricao,
            doc.MetaFinanceira,
            doc.ValorArrecadado,
            doc.Status,
            doc.DataInicio,
            doc.DataFim,
            doc.DataEncerramento,
            doc.Doacoes
                .Select(x => new DoacaoAnonimaDto(x.ApelidoDoador, x.Valor, x.Data))
                .ToList());
    }

    internal static CampanhaListaDocument CriarListaDocument(CriarCampanhaProjectionInput campanha) =>
        new()
        {
            IdCampanha = campanha.IdCampanha,
            Titulo = campanha.Titulo,
            MetaFinanceira = campanha.MetaFinanceira,
            ValorArrecadado = 0m,
            Status = campanha.Status,
            DataInicio = campanha.DataInicio,
            DataFim = campanha.DataFim,
            DataEncerramento = null
        };

    internal static CampanhaDetalheDocument CriarDetalheDocument(CriarCampanhaProjectionInput campanha) =>
        new()
        {
            IdCampanha = campanha.IdCampanha,
            Titulo = campanha.Titulo,
            Descricao = campanha.Descricao,
            MetaFinanceira = campanha.MetaFinanceira,
            ValorArrecadado = 0m,
            Status = campanha.Status,
            DataInicio = campanha.DataInicio,
            DataFim = campanha.DataFim,
            DataEncerramento = null,
            Doacoes = []
        };
}
