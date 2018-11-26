using Acme.Repository.Base;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Repository.Subsidy.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Acme.Seps.Repository.Subsidy
{
    public class ParameterContext : BaseContext, ISepsRepository
    {
        public ParameterContext(DbContextOptions<BaseContext> options) : base(options) { }

        TAggregateRoot ISepsRepository.GetLatest<TAggregateRoot>() =>
            Set<TAggregateRoot>()
                .OrderByDescending(art => art.Period.ValidFrom)
                .FirstOrDefault();

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