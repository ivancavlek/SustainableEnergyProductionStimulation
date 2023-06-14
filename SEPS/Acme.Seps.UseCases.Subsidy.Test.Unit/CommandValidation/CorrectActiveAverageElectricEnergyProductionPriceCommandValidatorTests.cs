using Acme.Seps.UseCases.Subsidy.Command.Validation;
using FluentValidation.TestHelper;

namespace Acme.Seps.UseCases.Subsidy.Test.Unit.CommandValidation;

public class CorrectActiveAverageElectricEnergyProductionPriceCommandValidatorTests
{
    private readonly CorrectActiveAverageElectricEnergyProductionPriceCommandValidator _validator;

    public CorrectActiveAverageElectricEnergyProductionPriceCommandValidatorTests() => _validator = new CorrectActiveAverageElectricEnergyProductionPriceCommandValidator();

    public void ValidatorShouldHaveAnErrorOnAmount() => _validator.ShouldHaveValidationErrorFor(vlr => vlr.Amount, -2);

    public void ValidatorShouldHaveAnErrorOnRemark() => _validator.ShouldHaveValidationErrorFor(vlr => vlr.Remark, null as string);

    public void ValidatorShouldHaveAnErrorOnYear() => _validator.ShouldHaveValidationErrorFor(vlr => vlr.Year, 2002);

    public void ValidatorShouldHaveAnErrorOnMonth() => _validator.ShouldHaveValidationErrorFor(vlr => vlr.Month, 15);
}