using Acme.Seps.Domain.Subsidy.Entity;
using System;

namespace Acme.Seps.Domain.Subsidy.DomainService
{
    public sealed class CogenerationParameterService : ICogenerationParameterService
    {
        private const decimal _factor = 0.25M;
        private const decimal _initialNgspp = 1.07M;
        private const decimal _initialYaep = 0.2625M;

        decimal ICogenerationParameterService.Calculate(
            YearlyAverageElectricEnergyProductionPrice yaep,
            NaturalGasSellingPrice naturalGasSellingPrice) =>
            Math.Round(
                CalculateYaepRate(yaep) + CalculateNgspRate(naturalGasSellingPrice),
                4,
                MidpointRounding.AwayFromZero);

        private static decimal CalculateYaepRate(YearlyAverageElectricEnergyProductionPrice yaep) =>
            _factor * (yaep.Amount / _initialYaep);

        private static decimal CalculateNgspRate(NaturalGasSellingPrice ngsp) =>
            (1 - _factor) * (ngsp.Amount / _initialNgspp);
    }

    public interface ICogenerationParameterService
    {
        decimal Calculate(
            YearlyAverageElectricEnergyProductionPrice yaep, NaturalGasSellingPrice naturalGasSellingPrice);
    }
}