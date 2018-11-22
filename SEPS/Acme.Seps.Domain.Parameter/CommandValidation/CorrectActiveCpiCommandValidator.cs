using Acme.Seps.Domain.Parameter.Command;
using FluentValidation;

namespace Acme.Seps.Domain.Parameter.CommandValidation
{
    public sealed class CorrectActiveCpiCommandValidator : AbstractValidator<CorrectActiveCpiCommand>
    {
        public CorrectActiveCpiCommandValidator()
        {
            RuleFor(cac => cac.Amount)
                .GreaterThan(0M)
                .WithMessage(Infrastructure.Parameter.ParameterAmountBelowOrZeroException);
            RuleFor(cac => cac.Remark)
                .NotEmpty()
                .WithMessage(Infrastructure.Parameter.RemarkNotSetException);
        }
    }
}