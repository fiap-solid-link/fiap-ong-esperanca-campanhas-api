using System;

namespace Fiap.OngEsperanca.Campanhas.Api.Domain.Events;

// Evento que publicaremos quando o doador enviar a intenção
public record DoacaoRecebidaEvent(
    Guid CampanhaId,
    Guid DoadorId,
    decimal Valor,
    DateTime DataHoraIntencao
);