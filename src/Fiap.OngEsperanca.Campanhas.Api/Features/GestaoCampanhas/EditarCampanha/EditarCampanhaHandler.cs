using System.Threading;
using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.EditarCampanha;

public sealed class EditarCampanhaHandler(
    ICampanhaRepository campanhaRepository,
    CampanhasDbContext dbContext) : IRequestHandler<EditarCampanhaCommand, Result<string>>
{
    public async Task<Result<string>> Handle(EditarCampanhaCommand request, CancellationToken ct)
    {
        var campanha = await campanhaRepository.ObterPorIdAsync(request.Id, ct);
        if (campanha is null) return Result<string>.Fail("Campanha não encontrada.", 404);

        campanha.Editar(request.Titulo, request.Descricao, request.MetaFinanceira);

        await campanhaRepository.AtualizarAsync(campanha, ct);
        await dbContext.SaveChangesAsync(ct);

        return Result<string>.Ok("Campanha editada com sucesso.");
    }
}