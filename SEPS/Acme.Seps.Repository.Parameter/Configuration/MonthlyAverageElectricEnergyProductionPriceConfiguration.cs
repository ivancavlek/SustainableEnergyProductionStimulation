using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    public class MonthlyAverageElectricEnergyProductionPriceConfiguration
        : IEntityTypeConfiguration<MonthlyAverageElectricEnergyProductionPrice>
    {
        public void Configure(EntityTypeBuilder<MonthlyAverageElectricEnergyProductionPrice> builder)
        {
            builder.OwnsOne(vte => vte.MonthlyPeriod, vte =>
            {
                vte.Property(ppy => ppy.ValidFrom).HasColumnName("ValidFrom").IsRequired();
                vte.Property(ppy => ppy.ValidTill).HasColumnName("ValidTill");
            });
        }
    }
}