using System;
using Fiap.OngEsperanca.Campanhas.Api._Shared.Results;
using MediatR;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CancelarCampanha;

// O comando precisa apenas do ID da campanha que o Gestor quer cancelar
public record CancelarCampanhaCommand(Guid Id) : IRequest<Result<string>>;