using Acme.Seps.Domain.Parameter.Command;
using FluentValidation;

namespace Acme.Seps.Domain.Parameter.CommandValidation
{
    public sealed class CalculateNaturalGasCommandValidator : AbstractValidator<CalculateNaturalGasCommand>
    {
        public CalculateNaturalGasCommandValidator()
        {
            RuleFor(cng => cng.Amount)
                .GreaterThan(0M)
                .WithMessage(Infrastructure.Parameter.ParameterAmountBelowOrZeroException);
            RuleFor(cng => cng.Remark)
                .NotEmpty()
                .WithMessage(Infrastructure.Parameter.RemarkNotSetException);
        }
    }
}