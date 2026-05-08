using System.Security.Claims;
using Esperanca.Campanha.Infrastructure._Shared;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.Infrastructure.CurrentUser;

public class CurrentUserAccessorTest
{
    // ===== UserId =====

    [Fact]
    public void UserId_WhenNameIdentifierClaimPresent_ThenReturnsGuidFromClaim()
    {
        // Arrange
        var userId = Guid.Parse("a1b2c3d4-0000-0000-0000-000000000001");
        var accessor = BuildAccessor(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

        // Act
        var result = accessor.UserId;

        // Assert
        result.ShouldBe(userId);
    }

    [Fact]
    public void UserId_WhenSubClaimPresent_ThenReturnsGuidFromSub()
    {
        // Arrange
        var userId = Guid.Parse("a1b2c3d4-0000-0000-0000-000000000002");
        var accessor = BuildAccessor(new Claim("sub", userId.ToString()));

        // Act
        var result = accessor.UserId;

        // Assert
        result.ShouldBe(userId);
    }

    [Fact]
    public void UserId_WhenNoIdentityClaim_ThenReturnsEmptyGuid()
    {
        // Arrange
        var accessor = BuildAccessor();

        // Act
        var result = accessor.UserId;

        // Assert
        result.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void UserId_WhenHttpContextIsNull_ThenReturnsEmptyGuid()
    {
        // Arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns((HttpContext?)null);
        var accessor = new CurrentUserAccessor(httpContextAccessor);

        // Act
        var result = accessor.UserId;

        // Assert
        result.ShouldBe(Guid.Empty);
    }

    // ===== Roles =====

    [Fact]
    public void Roles_WhenRoleClaimsPresent_ThenReturnsAllRoles()
    {
        // Arrange
        var accessor = BuildAccessor(
            new Claim(ClaimTypes.Role, "GestorONG"),
            new Claim(ClaimTypes.Role, "Doador"));

        // Act
        var result = accessor.Roles;

        // Assert
        result.Count.ShouldBe(2);
        result.ShouldContain("GestorONG");
        result.ShouldContain("Doador");
    }

    [Fact]
    public void Roles_WhenNoRoleClaims_ThenReturnsEmptyList()
    {
        // Arrange
        var accessor = BuildAccessor();

        // Act
        var result = accessor.Roles;

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void Roles_WhenHttpContextIsNull_ThenReturnsEmptyList()
    {
        // Arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns((HttpContext?)null);
        var accessor = new CurrentUserAccessor(httpContextAccessor);

        // Act
        var result = accessor.Roles;

        // Assert
        result.ShouldBeEmpty();
    }

    // ===== EstaNaRole =====

    [Fact]
    public void EstaNaRole_WhenUserHasRole_ThenReturnsTrue()
    {
        // Arrange
        var accessor = BuildAccessor(new Claim(ClaimTypes.Role, "GestorONG"));

        // Act
        var result = accessor.EstaNaRole("GestorONG");

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void EstaNaRole_WhenUserDoesNotHaveRole_ThenReturnsFalse()
    {
        // Arrange
        var accessor = BuildAccessor(new Claim(ClaimTypes.Role, "Doador"));

        // Act
        var result = accessor.EstaNaRole("GestorONG");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void EstaNaRole_WhenHttpContextIsNull_ThenReturnsFalse()
    {
        // Arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns((HttpContext?)null);
        var accessor = new CurrentUserAccessor(httpContextAccessor);

        // Act
        var result = accessor.EstaNaRole("GestorONG");

        // Assert
        result.ShouldBeFalse();
    }

    // ===== Helper =====

    private static CurrentUserAccessor BuildAccessor(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(principal);
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(httpContext);
        return new CurrentUserAccessor(httpContextAccessor);
    }
}
