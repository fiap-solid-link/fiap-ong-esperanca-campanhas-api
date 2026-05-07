namespace Esperanca.Campanha.Domain._Shared;

public class DomainException : Exception
{
    public string Codigo { get; }

    public DomainException(string codigo, string mensagem) : base(mensagem)
    {
        Codigo = codigo;
    }
}
