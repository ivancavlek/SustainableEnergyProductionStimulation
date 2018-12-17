using Acme.Seps.Domain.Subsidy.Command.Validation;
using FluentValidation.TestHelper;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.CommandValidation
{
    public class CalculateCpiCommandValidatorTests
    {
        private readonly CalculateConsumerPriceIndexCommandValidator _validator;

        public CalculateCpiCommandValidatorTests()
        {
            _validator = new CalculateConsumerPriceIndexCommandValidator();
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