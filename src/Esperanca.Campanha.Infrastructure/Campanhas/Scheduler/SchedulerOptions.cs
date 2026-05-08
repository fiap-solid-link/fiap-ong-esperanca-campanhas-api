namespace Esperanca.Campanha.Infrastructure.Campanhas.Scheduler;

public sealed class SchedulerOptions
{
    public const string SectionName = "Scheduler";

    public int IntervaloEmSegundos { get; init; } = 60;
    public int ProximidadeVencimentoEmDias { get; init; } = 3;
}
