using Esperanca.Campanha.Domain.Campanhas;
using FluentValidation;

namespace Esperanca.Campanha.Application.Campanhas.Listar;

public sealed class ListarCampanhasGestorValidator : AbstractValidator<ListarCampanhasGestorQuery>
{
    public const int TamanhoPaginaMaximo = 100;

    public ListarCampanhasGestorValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, TamanhoPaginaMaximo);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);

        RuleFor(x => x)
            .Must(q => q.DataInicioDe!.Value <= q.DataInicioAte!.Value)
            .When(q => q.DataInicioDe.HasValue && q.DataInicioAte.HasValue)
            .WithMessage(CampanhaErros.DataInicioMaiorQueDataFim);
    }
}
