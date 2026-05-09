using Esperanca.Campanha.WebApi._Shared.Middleware;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi._Shared.Middleware.CorrelationId;

public class CorrelationIdMiddlewareTest
{
    private const string HeaderName = "X-Correlation-Id";

    [Fact]
    public async Task InvokeAsync_WhenRequestHasCorrelationIdHeader_ThenUsesExistingIdNoItems()
    {
        // Arrange
        const string existingId = "existing-correlation-abc123";
        var context = new DefaultHttpContext();
        context.Request.Headers[HeaderName] = existingId;
        var nextCalled = false;
        var middleware = new CorrelationIdMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Items[HeaderName].ShouldBe(existingId);
        nextCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WhenRequestHasNoCorrelationIdHeader_ThenGeneratesIdNoFormatoGuidN()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextCalled = false;
        var middleware = new CorrelationIdMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var correlationId = context.Items[HeaderName]?.ToString();
        correlationId.ShouldNotBeNullOrEmpty();
        Guid.TryParseExact(correlationId, "N", out _).ShouldBeTrue();
        nextCalled.ShouldBeTrue();
    }
}
