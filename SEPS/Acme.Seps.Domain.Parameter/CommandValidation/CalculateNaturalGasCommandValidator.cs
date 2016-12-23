using Acme.Seps.Domain.Parameter.Command;
using FluentValidation;

namespace Acme.Seps.Domain.Parameter.CommandValidation
{
    public class CalculateNaturalGasCommandValidator : AbstractValidator<CalculateNaturalGasCommand>
    {
        public CalculateNaturalGasCommandValidator()
        {
            RuleFor(customer => customer.ActiveNaturalGasSellingPrice)
                .NotNull()
                .WithMessage(Infrastructure.Parameter.NaturalGasSellingPriceNotSetException);
            RuleForEach(customer => customer.ActiveCogenerationTariffs)
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