using Acme.Seps.Text;
using Acme.Seps.Text;
using FluentValidation;

namespace Acme.Seps.UseCases.Subsidy.Command.Validation
{
    public sealed class CalculateConsumerPriceIndexCommandValidator
        : AbstractValidator<CalculateConsumerPriceIndexCommand>
    {
        public CalculateConsumerPriceIndexCommandValidator()
        {
            RuleFor(ccc => ccc.Amount)
                .GreaterThan(0M)
                .WithMessage(SubsidyMessages.ParameterAmountBelowOrZeroException);
            RuleFor(ccc => ccc.Remark)
                .NotEmpty()
                .WithMessage(SubsidyMessages.RemarkNotSetException);
        }
    }
}