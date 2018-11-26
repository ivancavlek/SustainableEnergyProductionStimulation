using Acme.Seps.Domain.Subsidy.Entity;
using Microsoft.EntityFrameworkCore;

namespace Acme.Seps.Repository.Subsidy.Configuration
{
    internal sealed class MonthlyAverageElectricEnergyProductionPriceConfiguration
        : BaseParameterConfiguration<MonthlyAverageElectricEnergyProductionPrice>,
        IEntityTypeConfiguration<MonthlyAverageElectricEnergyProductionPrice>
    {
    }
}