using System;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.ProrrogarCampanha;

public record ProrrogarCampanhaCommand(Guid Id, DateTime NovaDataFim) : IRequest<Result<string>>;