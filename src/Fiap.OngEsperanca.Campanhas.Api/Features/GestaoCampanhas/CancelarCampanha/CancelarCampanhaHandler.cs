using System.Threading;
using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CancelarCampanha;

public sealed class CancelarCampanhaHandler(
    ICampanhaRepository campanhaRepository,
    CampanhasDbContext dbContext) : IRequestHandler<CancelarCampanhaCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CancelarCampanhaCommand request, CancellationToken ct)
    {
        // 1. Busca a campanha pelo ID no banco
        var campanha = await campanhaRepository.ObterPorIdAsync(request.Id, ct);

        // 2. Se não achar, retorna o nosso Result padronizado com erro 404
        if (campanha is null)
            return Result<string>.Fail("Campanha não encontrada.", 404);

        // 3. Aplica a regra de negócio do Domínio
        campanha.Cancelar();

        // 4. Diz para o repositório que houve atualização e salva
        await campanhaRepository.AtualizarAsync(campanha, ct);
        await dbContext.SaveChangesAsync(ct);

        // 5. Retorna sucesso
        return Result<string>.Ok("Campanha cancelada com sucesso.");
    }
}