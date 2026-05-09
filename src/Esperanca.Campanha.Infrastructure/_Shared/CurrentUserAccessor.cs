using System.Security.Claims;
using Esperanca.Campanha.Application._Shared;
using Microsoft.AspNetCore.Http;

namespace Esperanca.Campanha.Infrastructure._Shared;

public class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var sub = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? httpContextAccessor.HttpContext?.User.FindFirstValue("sub");
            return sub is not null ? Guid.Parse(sub) : Guid.Empty;
        }
    }

    public IReadOnlyList<string> Roles =>
        httpContextAccessor.HttpContext?.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? [];

    public bool EstaNaRole(string role) =>
        httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
}
