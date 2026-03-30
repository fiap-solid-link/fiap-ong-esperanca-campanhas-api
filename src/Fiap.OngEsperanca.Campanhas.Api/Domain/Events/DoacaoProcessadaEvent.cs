using System;

namespace Fiap.OngEsperanca.Campanhas.Api.Domain.Events;

// Evento que consumiremos do Worker para atualizar o saldo
public record DoacaoProcessadaEvent(
    Guid DoacaoId,
    Guid CampanhaId,
    decimal ValorProcessado,
    bool Sucesso,
    DateTime DataHoraProcessamento
);