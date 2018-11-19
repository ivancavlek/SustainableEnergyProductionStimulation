using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    internal sealed class MonthlyAverageElectricEnergyProductionPriceConfiguration
        : BaseParameterConfiguration<MonthlyAverageElectricEnergyProductionPrice>,
        IEntityTypeConfiguration<MonthlyAverageElectricEnergyProductionPrice>
    {
    }
}