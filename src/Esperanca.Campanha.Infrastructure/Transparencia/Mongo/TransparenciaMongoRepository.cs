using Esperanca.Campanha.Application.Transparencia._Shared;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Esperanca.Campanha.Infrastructure.Transparencia.Mongo;

public sealed class TransparenciaMongoRepository(
    IMongoClient mongoClient,
    IOptions<TransparenciaMongoOptions> options)
    : ITransparenciaReadRepository
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

    public async Task<IReadOnlyList<CampanhaTransparenciaDto>> ListarCampanhasAsync(CancellationToken cancellationToken = default)
    {
        var collection = _database.GetCollection<CampanhaListaDocument>(_opts.ListaCampanhasCollection);

        var docs = await collection
            .Find(FilterDefinition<CampanhaListaDocument>.Empty)
            .ToListAsync(cancellationToken);

        return docs
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
    }

    public async Task<CampanhaDetalheDto?> ObterDetalheCampanhaAsync(Guid idCampanha, CancellationToken cancellationToken = default)
    {
        var collection = _database.GetCollection<CampanhaDetalheDocument>(_opts.CampanhaDetalheCollection);

        var doc = await collection
            .Find(d => d.IdCampanha == idCampanha)
            .FirstOrDefaultAsync(cancellationToken);

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
}
