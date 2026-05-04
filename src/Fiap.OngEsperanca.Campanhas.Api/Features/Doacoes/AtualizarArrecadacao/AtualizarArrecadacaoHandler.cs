using System.Threading;
using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.Doacoes.AtualizarArrecadacao;

public sealed class AtualizarArrecadacaoHandler(
    ICampanhaRepository campanhaRepository,
    CampanhasDbContext dbContext) : IRequestHandler<AtualizarArrecadacaoCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AtualizarArrecadacaoCommand request, CancellationToken ct)
    {
        var campanha = await campanhaRepository.ObterPorIdAsync(request.CampanhaId, ct);

        if (campanha is null)
            return Result<string>.Fail("Campanha não encontrada para atualizar a arrecadação.", 404);

        // Chama o método que criamos lá na entidade de Domínio!
        campanha.AdicionarArrecadacao(request.Valor);

        await campanhaRepository.AtualizarAsync(campanha, ct);
        await dbContext.SaveChangesAsync(ct);

        return Result<string>.Ok("Arrecadação atualizada com sucesso.");
    }
}