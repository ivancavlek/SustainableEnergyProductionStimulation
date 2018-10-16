using Acme.Repository.Base;
using Acme.Seps.Domain.Parameter.Entity;
using Acme.Seps.Repository.Parameter.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Acme.Seps.Repository.Parameter
{
    public class ParameterContext : BaseContext
    {
        public DbSet<EconometricIndex> EconometricIndices { get { return Set<EconometricIndex>(); } }
        public DbSet<Tariff> Tariffs { get { return Set<Tariff>(); } }

        public ParameterContext(DbContextOptions<BaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("parameter");
            modelBuilder.ApplyConfiguration(new ConsumerPriceIndexConfiguration());
            modelBuilder.ApplyConfiguration(new MonthlyAverageElectricEnergyProductionPriceConfiguration());
            modelBuilder.ApplyConfiguration(new NaturalGasSellingPriceConfiguration());
            modelBuilder.ApplyConfiguration(new CogenerationTariffConfiguration());
            modelBuilder.ApplyConfiguration(new RenewableEnergySourceTariffConfiguration());
            modelBuilder.ApplyConfiguration(new SepsBaseEntityConfiguration());
        }
    }
}