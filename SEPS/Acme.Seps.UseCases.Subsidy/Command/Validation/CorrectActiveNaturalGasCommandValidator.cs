using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.UseCases.Subsidy.Command.Infrastructure;
using FluentValidation;

namespace Acme.Seps.UseCases.Subsidy.Command.Validation
{
    public sealed class CorrectActiveNaturalGasCommandValidator
        : AbstractValidator<CorrectActiveNaturalGasSellingPriceCommand>
    {
        public CorrectActiveNaturalGasCommandValidator()
        {
            RuleFor(can => can.Amount)
                .GreaterThan(0M)
                .WithMessage(SubsidyMessages.ParameterAmountBelowOrZeroException);
            RuleFor(can => can.Remark)
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