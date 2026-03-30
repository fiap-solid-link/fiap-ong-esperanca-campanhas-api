using FluentValidation;

namespace Fiap.OngEsperanca.Campanhas.Api.Features.GestaoCampanhas.CriarCampanha;

public class CriarCampanhaValidator : AbstractValidator<CriarCampanhaCommand>
{
    public CriarCampanhaValidator()
    {
        RuleFor(c => c.Titulo)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(150).WithMessage("O título deve ter no máximo 150 caracteres.");

        RuleFor(c => c.Descricao)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

        RuleFor(c => c.DataInicio)
            .NotEmpty().WithMessage("A data de início é obrigatória.");

        RuleFor(c => c.DataFim)
            .NotEmpty().WithMessage("A data de término é obrigatória.")
            .GreaterThan(c => c.DataInicio).WithMessage("A data de término deve ser posterior à data de início.");

        RuleFor(c => c.MetaFinanceira)
            .GreaterThan(0).WithMessage("A meta financeira deve ser maior que zero.");
    }
}