using System.Threading;
using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CriarCampanha;

public sealed class CriarCampanhaHandler(
    ICampanhaRepository campanhaRepository,
    CampanhasDbContext dbContext) : IRequestHandler<CriarCampanhaCommand, Result<CriarCampanhaResponse>>
{
    public async Task<Result<CriarCampanhaResponse>> Handle(CriarCampanhaCommand request, CancellationToken ct)
    {
        // 1. Instancia a entidade (as validações de domínio rodam no construtor)
        var campanha = new Campanha(
            request.Titulo,
            request.Descricao,
            request.DataInicio,
            request.DataFim,
            request.MetaFinanceira);

        // 2. Adiciona ao repositório
        await campanhaRepository.AdicionarAsync(campanha, ct);

        // 3. Persiste no banco de dados relacional
        await dbContext.SaveChangesAsync(ct);

        // 4. Retorna o padrão Result.Created espelhado da identity-api
        return Result<CriarCampanhaResponse>.Created(
            new CriarCampanhaResponse(campanha.Id, campanha.Titulo));
    }
}