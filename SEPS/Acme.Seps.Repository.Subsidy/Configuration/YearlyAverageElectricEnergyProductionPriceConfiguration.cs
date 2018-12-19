using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Acme.Seps.Repository.Subsidy.Configuration
{
    internal sealed class YearlyAverageElectricEnergyProductionPriceConfiguration
        : BaseParameterConfiguration<YearlyAverageElectricEnergyProductionPrice>,
        IEntityTypeConfiguration<YearlyAverageElectricEnergyProductionPrice>
    {
        private readonly Guid _id;

        internal YearlyAverageElectricEnergyProductionPriceConfiguration(Guid id)
        {
            _id = id;
        }

        public override void Configure(EntityTypeBuilder<YearlyAverageElectricEnergyProductionPrice> builder)
        {
            base.Configure(builder);
            SeedData(builder);
        }

        private void SeedData(EntityTypeBuilder<YearlyAverageElectricEnergyProductionPrice> builder)
        {
            builder.HasData(
                new
                {
                    Id = _id,
                    Amount = 0.2625M,
                    Remark = "Initial value",
                    EconometricIndexType = nameof(YearlyAverageElectricEnergyProductionPrice)
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