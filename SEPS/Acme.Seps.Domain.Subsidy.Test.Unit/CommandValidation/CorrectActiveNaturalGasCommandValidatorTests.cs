using Acme.Seps.Domain.Subsidy.CommandValidation;
using FluentValidation.TestHelper;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.CommandValidation
{
    public class CorrectActiveNaturalGasCommandValidatorTests
    {
        private readonly CorrectActiveNaturalGasCommandValidator _validator;

        public CorrectActiveNaturalGasCommandValidatorTests()
        {
            _validator = new CorrectActiveNaturalGasCommandValidator();
        }

        public void ValidatorShouldHaveAnErrorOnAmount()
        {
            _validator.ShouldHaveValidationErrorFor(vlr => vlr.Amount, -2);
        }

        public void ValidatorShouldHaveAnErrorOnRemark()
        {
            _validator.ShouldHaveValidationErrorFor(vlr => vlr.Remark, null as string);
        }

        public void ValidatorShouldHaveAnErrorOnYear()
        {
            _validator.ShouldHaveValidationErrorFor(vlr => vlr.Year, 2002);
        }

        public void ValidatorShouldHaveAnErrorOnMonth()
        {
            _validator.ShouldHaveValidationErrorFor(vlr => vlr.Month, 15);
        }
    }
}