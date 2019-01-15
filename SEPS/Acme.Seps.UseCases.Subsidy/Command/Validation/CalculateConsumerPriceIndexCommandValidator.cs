using Acme.Seps.Text;
using FluentValidation;

namespace Acme.Seps.UseCases.Subsidy.Command.Validation
{
    public sealed class CalculateConsumerPriceIndexCommandValidator
        : AbstractValidator<CalculateNewConsumerPriceIndexCommand>
    {
        public CalculateConsumerPriceIndexCommandValidator()
        {
            RuleFor(ccc => ccc.Amount)
                .GreaterThan(0M)
                .WithMessage(ccc => SepsMessage.ValueZeroOrAbove(nameof(ccc.Amount)));
            RuleFor(ccc => ccc.Remark)
                .NotEmpty()
                .WithMessage(ccc => SepsMessage.EntityNotSet(nameof(ccc.Remark)));
        }
    }
}