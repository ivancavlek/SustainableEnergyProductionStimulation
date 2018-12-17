using Acme.Seps.UseCases.Subsidy.Command.Validation;
using FluentValidation.TestHelper;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.CommandValidation
{
    public class CalculateNaturalGasCommandValidatorTests
    {
        private readonly CalculateNaturalGasCommandValidator _validator;

        public CalculateNaturalGasCommandValidatorTests()
        {
            _validator = new CalculateNaturalGasCommandValidator();
        }

        public void ValidatorShouldHaveAnErrorOnAmount()
        {
            _validator.ShouldHaveValidationErrorFor(vlr => vlr.Amount, -2);
        }

        public void ValidatorShouldHaveAnErrorOnRemark()
        {
            _validator.ShouldHaveValidationErrorFor(vlr => vlr.Remark, null as string);
        }
    }
}