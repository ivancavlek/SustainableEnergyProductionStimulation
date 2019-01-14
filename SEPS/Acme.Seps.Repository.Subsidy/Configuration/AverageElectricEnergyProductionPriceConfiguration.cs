using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Acme.Seps.Repository.Subsidy.Configuration
{
    internal sealed class AverageElectricEnergyProductionPriceConfiguration
        : BaseParameterConfiguration<AverageElectricEnergyProductionPrice>,
        IEntityTypeConfiguration<AverageElectricEnergyProductionPrice>
    {
        private readonly Guid _id;

        internal AverageElectricEnergyProductionPriceConfiguration(Guid id)
        {
            _id = id;
        }

        public override void Configure(EntityTypeBuilder<AverageElectricEnergyProductionPrice> builder)
        {
            base.Configure(builder);
            SeedData(builder);
        }

        private void SeedData(EntityTypeBuilder<AverageElectricEnergyProductionPrice> builder)
        {
            builder.HasData(
                new
                {
                    Id = _id,
                    Amount = 0.2625M,
                    Remark = "Initial value",
                    EconometricIndexType = nameof(AverageElectricEnergyProductionPrice)
                });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    EconometricIndexId = _id,
                    Since = SepsVersion.InitialDate(),
                });
            });
        }
    }
}