using MediatR;

namespace Esperanca.Campanha.Application.Campanhas.EncerrarVencidas;

public record EncerrarCampanhasVencidasCommand(int ProximidadeVencimentoEmDias) : IRequest<Unit>;
