using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Text;
using FluentValidation;

namespace Acme.Seps.UseCases.Subsidy.Command.Validation
{
    public sealed class CalculateNaturalGasCommandValidator : AbstractValidator<CalculateNewNaturalGasSellingPriceCommand>
    {
        public CalculateNaturalGasCommandValidator()
        {
            RuleFor(cng => cng.Amount)
                .GreaterThan(0M)
                .WithMessage(cng => SepsMessage.ValueZeroOrAbove(nameof(cng.Amount)));
            RuleFor(cng => cng.Remark)
                .NotEmpty()
                .WithMessage(cng => SepsMessage.EntityNotSet(nameof(cng.Remark)));
            RuleFor(cng => cng.Year)
                .GreaterThan(SepsVersion.InitialDate().Year)
                .WithMessage(cng => SepsMessage.ValueHigherThanTheOther(cng.Year.ToString(), SepsVersion.InitialDate().Year.ToString()));
            RuleFor(cng => cng.Month)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(12)
                .WithMessage(cng => SepsMessage.ValueHigherThanTheOther(cng.Month.ToString(), "1 - 12"));
        }
    }
}