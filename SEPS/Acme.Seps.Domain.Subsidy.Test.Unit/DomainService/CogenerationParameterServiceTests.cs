using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Test.Unit.Utility.Factory;
using FluentAssertions;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.DomainService
{
    public class CogenerationParameterServiceTests
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly YearlyAverageElectricEnergyProductionPrice _yearlyAverageElectricEnergyProductionPrice;

        public CogenerationParameterServiceTests()
        {
            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(DateTime.Now.AddYears(-3));
            _naturalGasSellingPrice = ngspFactory.Create();

            IEconometricIndexFactory<YearlyAverageElectricEnergyProductionPrice> yaepFactory =
                new EconometricIndexFactory<YearlyAverageElectricEnergyProductionPrice>(DateTime.Now.AddYears(-3));
            _yearlyAverageElectricEnergyProductionPrice = yaepFactory.Create();

            _cogenerationParameterService = new CogenerationParameterService();
        }

        public void RatesAreCorrectlyCalculated()
        {
            var result = _cogenerationParameterService
                .Calculate(_yearlyAverageElectricEnergyProductionPrice, _naturalGasSellingPrice);

            result.Should().Be(165.3316M);
        }
    }
}