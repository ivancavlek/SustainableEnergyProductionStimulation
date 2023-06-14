using Acme.Seps.UseCases.Subsidy.Command.Validation;
using FluentValidation.TestHelper;

namespace Acme.Seps.UseCases.Subsidy.Test.Unit.CommandValidation;

public class CalculateCpiCommandValidatorTests
{
    private readonly CalculateConsumerPriceIndexCommandValidator _validator;

    public CalculateCpiCommandValidatorTests() => _validator = new CalculateConsumerPriceIndexCommandValidator();

    public void ValidatorShouldHaveAnErrorOnAmount() => _validator.ShouldHaveValidationErrorFor(vlr => vlr.Amount, -2);

    public void ValidatorShouldHaveAnErrorOnRemark() => _validator.ShouldHaveValidationErrorFor(vlr => vlr.Remark, null as string);
}