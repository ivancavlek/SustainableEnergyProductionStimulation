using Acme.Domain.Base.Factory;
using Acme.Repository.Base;
using Acme.Seps.Repository.Subsidy.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Repository.Subsidy
{
    public class ParameterContext : BaseContext
    {
        private readonly IIdentityFactory<Guid> _identityFactory;

        public ParameterContext(DbContextOptions<BaseContext> options, IIdentityFactory<Guid> identityFactory)
            : base(options)
        {
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cpiGuid = _identityFactory.CreateIdentity();
            var naturalGasSellingPriceGuid = _identityFactory.CreateIdentity();
            var monthlyAverageElectricEnergyProductionPriceGuid = _identityFactory.CreateIdentity();

            var projectTypeIds = new List<Guid>();

            for (int i = 0; i < 12; i++)
                projectTypeIds.Add(_identityFactory.CreateIdentity());

            modelBuilder.HasDefaultSchema("parameter");
            modelBuilder.ApplyConfiguration(new EconometricIndexConfiguration());
            modelBuilder.ApplyConfiguration(new ConsumerPriceIndexConfiguration(cpiGuid));
            modelBuilder.ApplyConfiguration(new MonthlyAverageElectricEnergyProductionPriceConfiguration(
                monthlyAverageElectricEnergyProductionPriceGuid));
            modelBuilder.ApplyConfiguration(new NaturalGasSellingPriceConfiguration(naturalGasSellingPriceGuid));
            modelBuilder.ApplyConfiguration(new ProjectTypeConfiguration(projectTypeIds));
            modelBuilder.ApplyConfiguration(new TariffConfiguration());
            modelBuilder.ApplyConfiguration(new CogenerationTariffConfiguration(
                naturalGasSellingPriceGuid, projectTypeIds, _identityFactory));
            modelBuilder.ApplyConfiguration(new RenewableEnergySourceTariffConfiguration(
                cpiGuid, projectTypeIds, _identityFactory));
        }
    }
}