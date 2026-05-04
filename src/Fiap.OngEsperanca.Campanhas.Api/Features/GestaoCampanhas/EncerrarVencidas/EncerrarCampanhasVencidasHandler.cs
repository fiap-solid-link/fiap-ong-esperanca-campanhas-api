using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.EncerrarVencidas;

public sealed class EncerrarCampanhasVencidasHandler(CampanhasDbContext dbContext)
    : IRequestHandler<EncerrarCampanhasVencidasCommand, Result<int>>
{
    public async Task<Result<int>> Handle(EncerrarCampanhasVencidasCommand request, CancellationToken ct)
    {
        // 1. Busca direto no EF Core todas as campanhas em andamento que já passaram da DataFim
        var campanhasVencidas = await dbContext.Campanhas
            .Where(c => c.Status == StatusCampanha.EmAndamento && c.DataFim <= DateTime.UtcNow)
            .ToListAsync(ct);

        // Se não tem nada vencido, avisa que alterou 0 campanhas
        if (!campanhasVencidas.Any())
            return Result<int>.Ok(0);

        // 2. Chama o método de domínio para cada uma
        foreach (var campanha in campanhasVencidas)
        {
            campanha.Encerrar();
        }

        // 3. Salva todas as alterações de uma vez só no banco!
        await dbContext.SaveChangesAsync(ct);

        return Result<int>.Ok(campanhasVencidas.Count);
    }
}