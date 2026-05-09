using Esperanca.Campanha.Application._Shared;
using Esperanca.Campanha.Domain.Campanhas;
using FluentValidation;

namespace Esperanca.Campanha.Application.Campanhas.Prorrogar;

public sealed class ProrrogarCampanhaValidator : AbstractValidator<ProrrogarCampanhaCommand>
{
    public ProrrogarCampanhaValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.NovaDataFim)
            .GreaterThan(_ => dateTimeProvider.UtcNow)
            .WithMessage(CampanhaErros.DataFimMenorQueAgora);
    }
}
