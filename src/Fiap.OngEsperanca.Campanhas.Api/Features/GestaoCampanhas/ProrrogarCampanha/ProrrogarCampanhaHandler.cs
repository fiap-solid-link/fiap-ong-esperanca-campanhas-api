using System.Threading;
using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.ProrrogarCampanha;

public sealed class ProrrogarCampanhaHandler(
    ICampanhaRepository campanhaRepository,
    CampanhasDbContext dbContext) : IRequestHandler<ProrrogarCampanhaCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ProrrogarCampanhaCommand request, CancellationToken ct)
    {
        var campanha = await campanhaRepository.ObterPorIdAsync(request.Id, ct);
        if (campanha is null) return Result<string>.Fail("Campanha não encontrada.", 404);

        campanha.Prorrogar(request.NovaDataFim);

        await campanhaRepository.AtualizarAsync(campanha, ct);
        await dbContext.SaveChangesAsync(ct);

        return Result<string>.Ok("Campanha prorrogada com sucesso.");
    }
}