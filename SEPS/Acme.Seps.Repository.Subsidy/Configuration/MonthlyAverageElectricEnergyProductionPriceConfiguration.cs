using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.UseCases.Subsidy.Command.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Acme.Seps.Repository.Subsidy.Configuration
{
    internal sealed class MonthlyAverageElectricEnergyProductionPriceConfiguration
        : BaseParameterConfiguration<MonthlyAverageElectricEnergyProductionPrice>,
        IEntityTypeConfiguration<MonthlyAverageElectricEnergyProductionPrice>
    {
        private readonly Guid _id;

        internal MonthlyAverageElectricEnergyProductionPriceConfiguration(Guid id)
        {
            _id = id;
        }

        public override void Configure(EntityTypeBuilder<MonthlyAverageElectricEnergyProductionPrice> builder)
        {
            base.Configure(builder);
            SeedData(builder);
        }

        private void SeedData(EntityTypeBuilder<MonthlyAverageElectricEnergyProductionPrice> builder)
        {
            builder.HasData(
                new
                {
                    Id = _id,
                    Amount = 0.2625M,
                    Remark = "Initial value",
                    EconometricIndexType = nameof(MonthlyAverageElectricEnergyProductionPrice)
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