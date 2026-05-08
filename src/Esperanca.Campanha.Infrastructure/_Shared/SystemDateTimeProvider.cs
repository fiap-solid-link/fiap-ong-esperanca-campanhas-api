using Esperanca.Campanha.Application._Shared;

namespace Esperanca.Campanha.Infrastructure._Shared;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
