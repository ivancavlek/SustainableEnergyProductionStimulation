using Acme.Seps.Domain.Subsidy.Command;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using FluentValidation;

namespace Acme.Seps.Domain.Subsidy.CommandValidation
{
    public sealed class CalculateCpiCommandValidator : AbstractValidator<CalculateCpiCommand>
    {
        public CalculateCpiCommandValidator()
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