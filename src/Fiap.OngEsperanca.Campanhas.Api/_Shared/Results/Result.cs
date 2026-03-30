namespace Fiap.OngEsperanca.Campanhas.Api._Shared.Results;

public class Result<T>
{
    public bool Sucesso { get; private set; }
    public T? Dados { get; private set; }
    public string? Erro { get; private set; }
    public int StatusCode { get; private set; }

    // Métodos estáticos auxiliares para criar respostas padronizadas
    public static Result<T> Created(T dados) => new() { Sucesso = true, Dados = dados, StatusCode = 201 };
    public static Result<T> Ok(T dados) => new() { Sucesso = true, Dados = dados, StatusCode = 200 };
    public static Result<T> Fail(string erro, int statusCode = 400) => new() { Sucesso = false, Erro = erro, StatusCode = statusCode };
    public static Result<T> Unauthorized(string erro) => new() { Sucesso = false, Erro = erro, StatusCode = 401 };
}