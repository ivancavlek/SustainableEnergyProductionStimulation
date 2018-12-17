using Acme.Seps.Domain.Subsidy.Command.Infrastructure;
using FluentValidation;

namespace Acme.Seps.Domain.Subsidy.Command.Validation
{
    public sealed class CorrectActiveConsumerPriceIndexCommandValidator
        : AbstractValidator<CorrectActiveConsumerPriceIndexCommand>
    {
        public CorrectActiveConsumerPriceIndexCommandValidator()
        {
            RuleFor(cac => cac.Amount)
                .GreaterThan(0M)
                .WithMessage(SubsidyMessages.ParameterAmountBelowOrZeroException);
            RuleFor(cac => cac.Remark)
                .NotEmpty()
                .WithMessage(SubsidyMessages.RemarkNotSetException);
        }
    }
}