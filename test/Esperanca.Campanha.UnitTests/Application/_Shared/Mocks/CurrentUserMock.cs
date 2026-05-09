using Esperanca.Campanha.Application._Shared;
using NSubstitute;

namespace Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

public class CurrentUserMock
{
    public static readonly Guid DefaultUserId = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301");

    public ICurrentUser Instance { get; }

    public CurrentUserMock()
    {
        Instance = Substitute.For<ICurrentUser>();
        SetupUserId(DefaultUserId);
    }

    public CurrentUserMock SetupUserId(Guid userId)
    {
        Instance.UserId.Returns(userId);
        return this;
    }
}
