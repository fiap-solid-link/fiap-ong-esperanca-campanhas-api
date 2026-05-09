using Esperanca.Campanha.Application._Shared;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

public class DateTimeProviderMock
{
    public static readonly DateTime DefaultNow = new(2026, 5, 8, 12, 0, 0, DateTimeKind.Utc);

    public IDateTimeProvider Instance { get; }

    public DateTimeProviderMock()
    {
        Instance = Substitute.For<IDateTimeProvider>();
        SetupNow(DefaultNow);
    }

    public DateTimeProviderMock SetupNow(DateTime now)
    {
        Instance.UtcNow.Returns(now);
        return this;
    }
}
