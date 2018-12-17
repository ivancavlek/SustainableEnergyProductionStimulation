using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.UseCases.Subsidy.Command.Infrastructure;
using FluentValidation;

namespace Acme.Seps.UseCases.Subsidy.Command.Validation
{
    public sealed class CalculateNaturalGasCommandValidator : AbstractValidator<CalculateNaturalGasSellingPriceCommand>
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