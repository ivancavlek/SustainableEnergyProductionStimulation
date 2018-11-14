using Acme.Seps.Domain.Parameter.CommandValidation;
using FluentValidation.TestHelper;

namespace Acme.Seps.Domain.Parameter.Test.Unit.CommandValidation
{
    public class CorrectActiveCpiCommandValidatorTests
    {
        private readonly CorrectActiveCpiCommandValidator _validator;

        public CorrectActiveCpiCommandValidatorTests()
        {
            _validator = new CorrectActiveCpiCommandValidator();
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