using Esperanca.Campanha.Domain.Campanhas;
using Esperanca.Campanha.UnitTests.Application._Shared.Mocks;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.UnitTests.Application.Campanhas.Listar.Fakers;

public static class CampanhaFaker
{
    public static readonly Guid IdGestor = CurrentUserMock.DefaultUserId;
    private static readonly DateTime Agora = DateTimeProviderMock.DefaultNow;

    public static CampanhaAgg Cadastrada(string titulo, DateTime dataInicio) =>
        Criar(titulo, dataInicio, IdGestor);

    public static CampanhaAgg DeOutroGestor(string titulo, DateTime dataInicio) =>
        Criar(titulo, dataInicio, Guid.NewGuid());

    public static CampanhaAgg EmAndamento(string titulo, DateTime dataInicio)
    {
        var campanha = Criar(titulo, dataInicio, IdGestor);
        campanha.Ativar();
        return campanha;
    }

    private static CampanhaAgg Criar(string titulo, DateTime dataInicio, Guid idGestor) =>
        CampanhaAgg.Criar(
            titulo,
            "Descrição válida",
            dataInicio,
            Agora.AddDays(60),
            1000m,
            ModoEncerramento.PorDataOuMeta,
            idGestor,
            Agora);
}
