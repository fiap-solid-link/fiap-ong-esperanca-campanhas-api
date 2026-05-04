using System;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.AtivarCampanha;

public record AtivarCampanhaCommand(Guid Id) : IRequest<Result<string>>;