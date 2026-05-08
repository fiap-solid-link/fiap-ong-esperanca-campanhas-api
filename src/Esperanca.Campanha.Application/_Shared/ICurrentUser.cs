namespace Esperanca.Campanha.Application._Shared;

public interface ICurrentUser
{
    Guid UserId { get; }
    IReadOnlyList<string> Roles { get; }
    bool EstaNaRole(string role);
}
