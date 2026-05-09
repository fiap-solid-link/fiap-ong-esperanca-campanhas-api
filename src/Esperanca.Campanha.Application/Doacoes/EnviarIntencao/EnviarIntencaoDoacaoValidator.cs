using Esperanca.Campanha.Domain.Campanhas;
using FluentValidation;

namespace Esperanca.Campanha.Application.Doacoes.EnviarIntencao;

public sealed class EnviarIntencaoDoacaoValidator : AbstractValidator<EnviarIntencaoDoacaoCommand>
{
    public EnviarIntencaoDoacaoValidator()
    {
        RuleFor(x => x.IdCampanha)
            .NotEmpty();

        RuleFor(x => x.Valor)
            .GreaterThan(0)
            .WithMessage(CampanhaErros.ArrecadacaoExigeValorPositivo);
    }
}
