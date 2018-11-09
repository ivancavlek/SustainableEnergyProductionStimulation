using Acme.Repository.Base;
using Acme.Seps.Repository.Parameter.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Acme.Seps.Repository.Parameter
{
    public class ParameterContext : BaseContext
    {
        public ParameterContext(DbContextOptions<BaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("parameter");
            modelBuilder.ApplyConfiguration(new EconometricIndexConfiguration());
            modelBuilder.ApplyConfiguration(new ConsumerPriceIndexConfiguration());
            modelBuilder.ApplyConfiguration(new MonthlyAverageElectricEnergyProductionPriceConfiguration());
            modelBuilder.ApplyConfiguration(new NaturalGasSellingPriceConfiguration());
            modelBuilder.ApplyConfiguration(new TariffConfiguration());
            modelBuilder.ApplyConfiguration(new CogenerationTariffConfiguration());
            modelBuilder.ApplyConfiguration(new RenewableEnergySourceTariffConfiguration());
        }
    }
}