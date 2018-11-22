using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.Entity;
using FluentValidation;

namespace Acme.Seps.Domain.Parameter.CommandValidation
{
    public sealed class CorrectActiveNaturalGasCommandValidator : AbstractValidator<CorrectActiveNaturalGasCommand>
    {
        public CorrectActiveNaturalGasCommandValidator()
        {
            RuleFor(can => can.Amount)
                .GreaterThan(0M)
                .WithMessage(Infrastructure.Parameter.ParameterAmountBelowOrZeroException);
            RuleFor(can => can.Remark)
                .NotEmpty()
                .WithMessage(Infrastructure.Parameter.RemarkNotSetException);
            RuleFor(can => can.Year)
                .GreaterThan(EconometricIndex.InitialPeriod.Year)
                .WithMessage(Infrastructure.Parameter.YearlyParameterException);
            RuleFor(can => can.Month)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(12)
                .WithMessage(Infrastructure.Parameter.MonthlyParameterException);
        }
    }
}