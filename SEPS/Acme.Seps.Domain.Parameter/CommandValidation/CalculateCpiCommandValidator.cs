using Acme.Seps.Domain.Parameter.Command;
using FluentValidation;

namespace Acme.Seps.Domain.Parameter.CommandValidation
{
    public class CalculateCpiCommandValidator : AbstractValidator<CalculateCpiCommand>
    {
        public CalculateCpiCommandValidator()
        {
            RuleFor(customer => customer.ActiveCpi)
                .NotNull()
                .WithMessage(Infrastructure.Parameter.ConsumerPriceIndexNotSetException);
            RuleForEach(customer => customer.ActiveResTariffs)
                .NotNull()
                .WithMessage(Infrastructure.Parameter.RemarkNotSetException);
            RuleFor(cfc => cfc.Amount)
                .GreaterThan(0M)
                .WithMessage(Infrastructure.Parameter.ParameterAmountBelowOrZeroException);
            RuleFor(customer => customer.Remark)
                .NotEmpty()
                .WithMessage(Infrastructure.Parameter.RemarkNotSetException);
        }
    }
}