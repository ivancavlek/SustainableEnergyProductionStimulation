using Acme.Seps.Domain.Subsidy.Entity;
using System;

namespace Acme.Seps.Domain.Subsidy.DomainService
{
    public sealed class CogenerationParameterService : ICogenerationParameterService
    {
        private const decimal _factor = 0.25M;
        private const decimal _initialNgspp = 1.07M;
        private const decimal _initialAeepp = 0.2625M;

        decimal ICogenerationParameterService.Calculate(
            AverageElectricEnergyProductionPrice averageElectricEnergyProductionPrice,
            NaturalGasSellingPrice naturalGasSellingPrice) =>
            Math.Round(
                CalculateAeeppRate(averageElectricEnergyProductionPrice) + CalculateNgspRate(naturalGasSellingPrice),
                4,
                MidpointRounding.AwayFromZero);

        private static decimal CalculateAeeppRate(AverageElectricEnergyProductionPrice aeepp) =>
            _factor * (aeepp.Amount / _initialAeepp);

        private static decimal CalculateNgspRate(NaturalGasSellingPrice ngsp) =>
            (1 - _factor) * (ngsp.Amount / _initialNgspp);
    }

    public interface ICogenerationParameterService
    {
        decimal Calculate(
            AverageElectricEnergyProductionPrice averageElectricEnergyProductionPrice,
            NaturalGasSellingPrice naturalGasSellingPrice);
    }
}