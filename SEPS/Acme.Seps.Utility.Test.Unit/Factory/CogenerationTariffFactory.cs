using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using NSubstitute;
using System;
using System.Reflection;

namespace Acme.Seps.Test.Unit.Utility.Factory
{
    public sealed class CogenerationTariffFactory : ICogenerationTariffFactory<CogenerationTariff>
    {
        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly AverageElectricEnergyProductionPrice _averageElectricEnergyProductionPrice;

        public CogenerationTariffFactory(
            AverageElectricEnergyProductionPrice averageElectricEnergyProductionPrice,
            NaturalGasSellingPrice naturalGasSellingPrice)
        {
            _naturalGasSellingPrice = naturalGasSellingPrice ??
                throw new ArgumentNullException(nameof(naturalGasSellingPrice));
            _averageElectricEnergyProductionPrice = averageElectricEnergyProductionPrice ??
                throw new ArgumentNullException(nameof(averageElectricEnergyProductionPrice));
        }

        CogenerationTariff ICogenerationTariffFactory<CogenerationTariff>.Create(DateTimeOffset activeSince) =>
            Activator.CreateInstance(
                typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[]
                {
                    _naturalGasSellingPrice,
                    _averageElectricEnergyProductionPrice,
                    100M,
                    500M,
                    10M,
                    10M,
                    activeSince,
                    Guid.NewGuid(),
                    Substitute.For<IIdentityFactory<Guid>>() },
                null) as CogenerationTariff;
    }

    public interface ICogenerationTariffFactory<TTariff> where TTariff : Tariff
    {
        TTariff Create(DateTimeOffset activeSince);
    }
}