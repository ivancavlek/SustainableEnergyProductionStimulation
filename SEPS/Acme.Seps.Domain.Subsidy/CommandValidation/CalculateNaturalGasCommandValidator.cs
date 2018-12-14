using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Command;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using FluentValidation;

namespace Acme.Seps.Domain.Subsidy.CommandValidation
{
    public sealed class CalculateNaturalGasCommandValidator : AbstractValidator<CalculateNaturalGasCommand>
    {
        public CalculateNaturalGasCommandValidator()
        {
            RuleFor(cng => cng.Amount)
                .GreaterThan(0M)
                .WithMessage(SubsidyMessages.ParameterAmountBelowOrZeroException);
            RuleFor(cng => cng.Remark)
                .NotEmpty()
                .WithMessage(SubsidyMessages.RemarkNotSetException);
            RuleFor(can => can.Year)
                .GreaterThan(SepsVersion.InitialDate().Year)
                .WithMessage(SubsidyMessages.YearlyParameterException);
            RuleFor(can => can.Month)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(12)
                .WithMessage(SubsidyMessages.MonthlyParameterException);
        }
    }
}