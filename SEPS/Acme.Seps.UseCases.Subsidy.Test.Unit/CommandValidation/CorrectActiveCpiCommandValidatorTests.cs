using Acme.Seps.UseCases.Subsidy.Command.Validation;
using FluentValidation.TestHelper;

namespace Acme.Seps.UseCases.Subsidy.Test.Unit.CommandValidation;

public class CorrectActiveCpiCommandValidatorTests
{
    private readonly CorrectActiveConsumerPriceIndexCommandValidator _validator;

    public CorrectActiveCpiCommandValidatorTests() => _validator = new CorrectActiveConsumerPriceIndexCommandValidator();

    public void ValidatorShouldHaveAnErrorOnAmount() => _validator.ShouldHaveValidationErrorFor(vlr => vlr.Amount, -2);

    public void ValidatorShouldHaveAnErrorOnRemark() => _validator.ShouldHaveValidationErrorFor(vlr => vlr.Remark, null as string);
}