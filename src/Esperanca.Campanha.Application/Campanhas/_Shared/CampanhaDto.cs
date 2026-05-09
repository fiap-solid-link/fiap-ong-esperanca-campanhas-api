using Esperanca.Campanha.Domain.Campanhas;

namespace Esperanca.Campanha.Application.Campanhas._Shared;

public record CampanhaDto(
    Guid Id,
    string Titulo,
    string Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    decimal MetaFinanceira,
    ModoEncerramento ModoEncerramento,
    StatusCampanha Status,
    decimal ValorArrecadado,
    Guid IdGestor)
{
    public static CampanhaDto From(Domain.Campanhas.Campanha c) =>
        new(c.Id, c.Titulo, c.Descricao, c.DataInicio, c.DataFim,
            c.MetaFinanceira, c.ModoEncerramento, c.Status, c.ValorArrecadado, c.IdGestor);
}
