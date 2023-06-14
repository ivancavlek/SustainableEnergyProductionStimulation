using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Test.Unit.Utility.Factory;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.DomainService;

public class CogenerationParameterServiceTests
{
    private readonly ICogenerationParameterService _cogenerationParameterService;
    private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
    private readonly AverageElectricEnergyProductionPrice _averageElectricEnergyProductionPrice;

    public CogenerationParameterServiceTests()
    {
        IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
            new EconometricIndexFactory<NaturalGasSellingPrice>(DateTime.Now.AddYears(-3));
        _naturalGasSellingPrice = ngspFactory.Create();

        IEconometricIndexFactory<AverageElectricEnergyProductionPrice> aeeppFactory =
            new EconometricIndexFactory<AverageElectricEnergyProductionPrice>(DateTime.Now.AddYears(-3));
        _averageElectricEnergyProductionPrice = aeeppFactory.Create();

        _cogenerationParameterService = new CogenerationParameterService();
    }

    public void RatesAreCorrectlyCalculated()
    {
        var result = _cogenerationParameterService
            .Calculate(_averageElectricEnergyProductionPrice, _naturalGasSellingPrice);

        result.Should().Be(165.3316M);
    }
}