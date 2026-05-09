using System.Text.Json;
using Esperanca.Campanha.WebApi._Shared.Middleware;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi._Shared.Middleware.ValidationException;

public class ValidationExceptionMiddlewareTest
{
    [Fact]
    public async Task InvokeAsync_WhenNoException_ThenCallsNextSemAlterarStatus()
    {
        // Arrange
        var nextCalled = false;
        var middleware = new ValidationExceptionMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });
        var context = BuildContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(200);
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationExceptionLancada_ThenReturn400ComEstruturaDeErros()
    {
        // Arrange
        var failures = new[] { new ValidationFailure("nome", "Campanha:001") };
        var middleware = new ValidationExceptionMiddleware(_ => throw new FluentValidation.ValidationException(failures));
        var context = BuildContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe(400);
        context.Response.ContentType.ShouldBe("application/json");

        var json = await ReadResponseJsonAsync(context);
        json.GetProperty("titulo").GetString().ShouldBe("Campanha:900");
        json.GetProperty("erros").GetProperty("nome")
            .EnumerateArray().First().GetString().ShouldBe("Campanha:001");
    }

    [Fact]
    public async Task InvokeAsync_WhenMultiplosErrosNaMesmaPropriedade_ThenAgrupaMensagensNaPropriedade()
    {
        // Arrange
        var failures = new[]
        {
            new ValidationFailure("nome", "Campanha:001"),
            new ValidationFailure("nome", "Campanha:002")
        };
        var middleware = new ValidationExceptionMiddleware(_ => throw new FluentValidation.ValidationException(failures));
        var context = BuildContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var json = await ReadResponseJsonAsync(context);
        var mensagens = json.GetProperty("erros").GetProperty("nome").EnumerateArray().ToList();
        mensagens.Count.ShouldBe(2);
        mensagens[0].GetString().ShouldBe("Campanha:001");
        mensagens[1].GetString().ShouldBe("Campanha:002");
    }

    [Fact]
    public async Task InvokeAsync_WhenMultiplasPropriedadesComErros_ThenTodasAsPropriedadesNoResponse()
    {
        // Arrange
        var failures = new[]
        {
            new ValidationFailure("nome", "Campanha:001"),
            new ValidationFailure("descricao", "Campanha:002")
        };
        var middleware = new ValidationExceptionMiddleware(_ => throw new FluentValidation.ValidationException(failures));
        var context = BuildContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var json = await ReadResponseJsonAsync(context);
        var erros = json.GetProperty("erros");
        erros.TryGetProperty("nome", out _).ShouldBeTrue();
        erros.TryGetProperty("descricao", out _).ShouldBeTrue();
    }

    private static DefaultHttpContext BuildContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<JsonElement> ReadResponseJsonAsync(DefaultHttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        return JsonSerializer.Deserialize<JsonElement>(body);
    }
}
