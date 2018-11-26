using Acme.Seps.Domain.Subsidy.Command;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using FluentValidation;

namespace Acme.Seps.Domain.Subsidy.CommandValidation
{
    public sealed class CorrectActiveCpiCommandValidator : AbstractValidator<CorrectActiveCpiCommand>
    {
        public CorrectActiveCpiCommandValidator()
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