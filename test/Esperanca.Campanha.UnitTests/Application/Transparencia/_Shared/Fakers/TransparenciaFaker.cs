using Esperanca.Campanha.Application.Transparencia._Shared;

namespace Esperanca.Campanha.UnitTests.Application.Transparencia._Shared.Fakers;

public static class TransparenciaFaker
{
    public static readonly DateTime Agora = new(2026, 5, 8, 12, 0, 0, DateTimeKind.Utc);

    public static PainelMacroDto PainelMacroComDados() =>
        new(
            TotalArrecadado: 4250m,
            TotalDoacoes: 12,
            TotalCampanhasAtivas: 1,
            TotalCampanhasConcluidas: 1,
            TopDoadores:
            [
                new("Ana M.", 1200m, 4),
                new("Bruno S.", 950m, 3),
                new("Carla R.", 700m, 2),
            ],
            AtualizadoEm: Agora);

    public static CampanhaTransparenciaDto CampanhaTransparencia(
        string status = "EmAndamento",
        string titulo = "Campanha A") =>
        new(
            Id: Guid.NewGuid(),
            Titulo: titulo,
            MetaFinanceira: 5000m,
            ValorArrecadado: 1200m,
            Status: status,
            DataInicio: Agora.AddDays(-30),
            DataFim: Agora.AddDays(30),
            DataEncerramento: status == "Concluida" ? Agora.AddDays(-1) : null);

    public static CampanhaDetalheDto DetalheCampanha(Guid id) =>
        new(
            Id: id,
            Titulo: "Campanha de Inverno",
            Descricao: "Arrecadação de cobertores",
            MetaFinanceira: 5000m,
            ValorArrecadado: 1200m,
            Status: "EmAndamento",
            DataInicio: Agora.AddDays(-30),
            DataFim: Agora.AddDays(30),
            DataEncerramento: null,
            Doacoes:
            [
                new("Ana M.", 500m, Agora.AddDays(-10)),
                new("Bruno S.", 250m, Agora.AddDays(-5)),
            ]);
}
