using Esperanca.Campanha.Infrastructure._Shared;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Infrastructure.DateTimeProvider;

public class SystemDateTimeProviderTest
{
    [Fact]
    public void UtcNow_Always_ReturnsUtcKind()
    {
        // Arrange
        var provider = new SystemDateTimeProvider();

        // Act
        var result = provider.UtcNow;

        // Assert
        result.Kind.ShouldBe(DateTimeKind.Utc);
    }

    [Fact]
    public void UtcNow_Always_ReturnsValueWithinSystemUtcRange()
    {
        // Arrange
        var provider = new SystemDateTimeProvider();
        var before = DateTime.UtcNow;

        // Act
        var result = provider.UtcNow;

        // Assert
        var after = DateTime.UtcNow;
        result.ShouldBeGreaterThanOrEqualTo(before);
        result.ShouldBeLessThanOrEqualTo(after);
    }
}
