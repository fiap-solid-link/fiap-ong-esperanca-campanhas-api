using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Domain.Campanhas;
using FluentValidation;

namespace Esperanca.Campanha.Application.Campanhas.Criar;

public sealed class CriarCampanhaValidator : AbstractValidator<CriarCampanhaCommand>
{
    public CriarCampanhaValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage(CampanhaErros.TituloObrigatorio);

        RuleFor(x => x.Descricao)
            .NotEmpty()
            .WithMessage(CampanhaErros.DescricaoObrigatoria);

        RuleFor(x => x.MetaFinanceira)
            .GreaterThan(0)
            .WithMessage(CampanhaErros.MetaFinanceiraInvalida);

        RuleFor(x => x.DataFim)
            .GreaterThan(_ => dateTimeProvider.UtcNow)
            .WithMessage(CampanhaErros.DataFimMenorQueAgora);

        RuleFor(x => x.DataInicio)
            .LessThanOrEqualTo(x => x.DataFim)
            .WithMessage(CampanhaErros.DataInicioMaiorQueDataFim);
    }
}
