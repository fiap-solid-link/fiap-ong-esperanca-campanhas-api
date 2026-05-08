using Esperanca.Campanha.Application.Campanhas.Editar;
using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Editar.Fakers;

public static class EditarCampanhaCommandFaker
{
    private static readonly DateTime Agora = DateTimeProviderMock.DefaultNow;

    public static EditarCampanhaCommand Valid(Guid id) =>
        new(id,
            "Novo Título da Campanha",
            "Nova descrição válida da campanha",
            Agora.AddDays(1),
            Agora.AddDays(60),
            2000m,
            ModoEncerramento.PorMeta);

    public static EditarCampanhaCommand ComDataFimNoPassado(Guid id) =>
        new(id,
            "Novo Título",
            "Nova descrição",
            Agora.AddDays(-5),
            Agora.AddDays(-1),
            1000m,
            ModoEncerramento.PorData);

    public static EditarCampanhaCommand ComTituloVazio(Guid id) =>
        new(id,
            "",
            "Nova descrição",
            Agora.AddDays(1),
            Agora.AddDays(30),
            1000m,
            ModoEncerramento.PorData);

    public static EditarCampanhaCommand ComMetaZero(Guid id) =>
        new(id,
            "Novo Título",
            "Nova descrição",
            Agora.AddDays(1),
            Agora.AddDays(30),
            0m,
            ModoEncerramento.PorData);

    public static EditarCampanhaCommand ComDataInicioAposDataFim(Guid id) =>
        new(id,
            "Novo Título",
            "Nova descrição",
            Agora.AddDays(20),
            Agora.AddDays(10),
            1000m,
            ModoEncerramento.PorData);
}
