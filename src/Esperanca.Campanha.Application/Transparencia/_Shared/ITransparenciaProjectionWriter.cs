namespace Esperanca.Campanha.Application.Transparencia._Shared
{
    public interface ITransparenciaProjectionWriter
    {
        Task CriarProjecaoCampanhaAsync(CriarCampanhaProjectionInput input, CancellationToken cancellationToken = default);

        Task AtualizarStatusCampanhaAsync(Guid idCampanha, string status, DateTime? dataEncerramento, CancellationToken cancellationToken = default);
    }
}
