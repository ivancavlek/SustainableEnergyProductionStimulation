using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using NSubstitute;
using System;
using System.Reflection;

namespace Acme.Seps.Test.Unit.Utility.Factory
{
    public sealed class CogenerationTariffFactory : ITariffFactory<CogenerationTariff>
    {
        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly YearlyAverageElectricEnergyProductionPrice _yearlyAverageElectricEnergyProductionPrice;

        public CogenerationTariffFactory(
            YearlyAverageElectricEnergyProductionPrice yearlyAverageElectricEnergyProductionPrice,
            NaturalGasSellingPrice naturalGasSellingPrice)
        {
            _naturalGasSellingPrice = naturalGasSellingPrice ??
                throw new ArgumentNullException(nameof(naturalGasSellingPrice));
            _yearlyAverageElectricEnergyProductionPrice = yearlyAverageElectricEnergyProductionPrice ??
                throw new ArgumentNullException(nameof(yearlyAverageElectricEnergyProductionPrice));
        }

        CogenerationTariff ITariffFactory<CogenerationTariff>.Create() =>
            Activator.CreateInstance(
                typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[]
                {
                    _naturalGasSellingPrice,
                    _yearlyAverageElectricEnergyProductionPrice,
                    100M,
                    500M,
                    10M,
                    10M,
                    Guid.NewGuid(),
                    Substitute.For<IIdentityFactory<Guid>>() },
                null) as CogenerationTariff;
    }
}