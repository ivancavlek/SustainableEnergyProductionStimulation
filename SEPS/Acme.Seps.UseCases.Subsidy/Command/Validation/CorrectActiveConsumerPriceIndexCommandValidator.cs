using Acme.Seps.Text;
using FluentValidation;

namespace Acme.Seps.UseCases.Subsidy.Command.Validation;

public sealed class CorrectActiveConsumerPriceIndexCommandValidator
    : AbstractValidator<CorrectActiveConsumerPriceIndexCommand>
{
    public CorrectActiveConsumerPriceIndexCommandValidator()
    {
        RuleFor(cac => cac.Amount)
            .GreaterThan(0M)
            .WithMessage(ccc => SepsMessage.ValueZeroOrAbove(nameof(ccc.Amount)));
        RuleFor(cac => cac.Remark)
            .NotEmpty()
            .WithMessage(ccc => SepsMessage.EntityNotSet(nameof(ccc.Remark)));
    }
}