using Esperanca.Campanha.Application.Doacoes.EnviarIntencao;

namespace Esperanca.Campanha.UnitTests.WebApi.Doacoes.EnviarIntencao.Fakers;

public static class IntencaoDoacaoDtoFaker
{
    private static readonly DateTime _data = new(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);

    public static IntencaoDoacaoDto Valid() =>
        new(Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
            Guid.Parse("bbbbbbbb-0000-0000-0000-000000000001"),
            _data);
}
