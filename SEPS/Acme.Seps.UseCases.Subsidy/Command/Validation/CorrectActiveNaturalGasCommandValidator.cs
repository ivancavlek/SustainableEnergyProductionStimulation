using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Text;
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
                .WithMessage(ccc => SepsMessage.ValueZeroOrAbove(nameof(ccc.Amount)));
            RuleFor(can => can.Remark)
                .NotEmpty()
                .WithMessage(ccc => SepsMessage.EntityNotSet(nameof(ccc.Remark)));
            RuleFor(can => can.Year)
                .GreaterThan(SepsVersion.InitialDate().Year)
                .WithMessage(can => SepsMessage.ValueHigherThanTheOther(can.Year.ToString(), SepsVersion.InitialDate().Year.ToString()));
            RuleFor(can => can.Month)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(12)
                .WithMessage(can => SepsMessage.ValueHigherThanTheOther(can.Month.ToString(), "1 - 12"));
        }
    }
}