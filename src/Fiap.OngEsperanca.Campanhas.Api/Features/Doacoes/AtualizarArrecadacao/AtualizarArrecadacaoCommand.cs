using System;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.Doacoes.AtualizarArrecadacao;

public record AtualizarArrecadacaoCommand(Guid CampanhaId, decimal Valor) : IRequest<Result<string>>;