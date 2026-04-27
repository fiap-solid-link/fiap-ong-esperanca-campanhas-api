using System;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.Doacoes.EnviarIntencao;

// O DoadorId viria do Token JWT na vida real, mas vamos receber no corpo para o MVP
public record EnviarIntencaoDoacaoCommand(
    Guid CampanhaId,
    Guid DoadorId,
    decimal Valor
) : IRequest<Result<string>>;