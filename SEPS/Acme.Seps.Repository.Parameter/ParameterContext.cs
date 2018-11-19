using Acme.Repository.Base;
using Acme.Seps.Repository.Parameter.Configuration;
using Microsoft.EntityFrameworkCore;
using System;

namespace Acme.Seps.Repository.Parameter
{
    public class ParameterContext : BaseContext
    {
        public ParameterContext(DbContextOptions<BaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cpiGuid = Guid.NewGuid();
            var naturalGasSellingPriceGuid = Guid.NewGuid();

            modelBuilder.HasDefaultSchema("parameter");
            modelBuilder.ApplyConfiguration(new EconometricIndexConfiguration());
            modelBuilder.ApplyConfiguration(new ConsumerPriceIndexConfiguration(cpiGuid));
            modelBuilder.ApplyConfiguration(new MonthlyAverageElectricEnergyProductionPriceConfiguration());
            modelBuilder.ApplyConfiguration(new NaturalGasSellingPriceConfiguration(naturalGasSellingPriceGuid));
            modelBuilder.ApplyConfiguration(new TariffConfiguration());
            modelBuilder.ApplyConfiguration(new CogenerationTariffConfiguration(naturalGasSellingPriceGuid));
            modelBuilder.ApplyConfiguration(new RenewableEnergySourceTariffConfiguration(cpiGuid));
        }
    }
}