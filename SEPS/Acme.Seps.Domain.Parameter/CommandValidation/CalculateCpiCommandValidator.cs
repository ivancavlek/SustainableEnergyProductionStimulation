using Acme.Seps.Domain.Parameter.Command;
using FluentValidation;

namespace Acme.Seps.Domain.Parameter.CommandValidation
{
    public sealed class CalculateCpiCommandValidator : AbstractValidator<CalculateCpiCommand>
    {
        public CalculateCpiCommandValidator()
        {
            RuleFor(cfc => cfc.Amount)
                .GreaterThan(0M)
                .WithMessage(Infrastructure.Parameter.ParameterAmountBelowOrZeroException);
            RuleFor(customer => customer.Remark)
                .NotEmpty()
                .WithMessage(Infrastructure.Parameter.RemarkNotSetException);
        }
    }
}