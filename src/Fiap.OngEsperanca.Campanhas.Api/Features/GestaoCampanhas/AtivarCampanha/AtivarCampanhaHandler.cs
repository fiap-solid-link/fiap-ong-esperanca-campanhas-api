using System.Threading;
using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.AtivarCampanha;

public sealed class AtivarCampanhaHandler(
    ICampanhaRepository campanhaRepository,
    CampanhasDbContext dbContext) : IRequestHandler<AtivarCampanhaCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AtivarCampanhaCommand request, CancellationToken ct)
    {
        var campanha = await campanhaRepository.ObterPorIdAsync(request.Id, ct);
        if (campanha is null) return Result<string>.Fail("Campanha não encontrada.", 404);

        // A regra de negócio está blindada lá no Domínio!
        campanha.Ativar();

        await campanhaRepository.AtualizarAsync(campanha, ct);
        await dbContext.SaveChangesAsync(ct);

        return Result<string>.Ok("Campanha ativada com sucesso.");
    }
}