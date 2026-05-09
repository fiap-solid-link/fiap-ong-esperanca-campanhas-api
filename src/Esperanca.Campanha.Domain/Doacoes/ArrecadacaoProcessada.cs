namespace Esperanca.Campanha.Domain.Doacoes;

public class ArrecadacaoProcessada
{
    public Guid IdDoacao { get; private set; }
    public Guid IdCampanha { get; private set; }
    public decimal Valor { get; private set; }
    public DateTime DataProcessamento { get; private set; }

    private ArrecadacaoProcessada() { }

    public static ArrecadacaoProcessada Registrar(
        Guid idDoacao,
        Guid idCampanha,
        decimal valor,
        DateTime dataProcessamento) =>
        new()
        {
            IdDoacao = idDoacao,
            IdCampanha = idCampanha,
            Valor = valor,
            DataProcessamento = dataProcessamento
        };
}
