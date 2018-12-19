using Acme.Seps.Domain.Subsidy.Entity;

namespace Acme.Seps.Domain.Subsidy.DomainService
{
    public interface ICogenerationParameterService
    {
        decimal Calculate(
            YearlyAverageElectricEnergyProductionPrice yaep, NaturalGasSellingPrice naturalGasSellingPrice);
    }
}