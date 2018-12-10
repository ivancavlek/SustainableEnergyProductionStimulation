using Acme.Seps.Domain.Subsidy.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.Seps.Repository.Subsidy.Configuration
{
    internal sealed class EconometricIndexConfiguration
        : BaseParameterConfiguration<EconometricIndex>, IEntityTypeConfiguration<EconometricIndex>
    {
        private readonly string _discriminator;

        internal EconometricIndexConfiguration()
        {
            _discriminator = "EconometricIndexType";
        }

        public override void Configure(EntityTypeBuilder<EconometricIndex> builder)
        {
            ConfigureProperties(builder);
            ConfigureTables(builder);

            base.Configure(builder);
        }

        private void ConfigureTables(EntityTypeBuilder<EconometricIndex> builder)
        {
            builder
                .ToTable("EconometricIndexes")
                .HasDiscriminator<string>(_discriminator)
                .HasValue<ConsumerPriceIndex>(nameof(ConsumerPriceIndex))
                .HasValue<NaturalGasSellingPrice>(nameof(NaturalGasSellingPrice))
                .HasValue<MonthlyAverageElectricEnergyProductionPrice>(
                    nameof(MonthlyAverageElectricEnergyProductionPrice));
        }

        private void ConfigureProperties(EntityTypeBuilder<EconometricIndex> builder)
        {
            builder.Property<string>(_discriminator).HasMaxLength(50);
            builder.Property(ppy => ppy.Remark).HasMaxLength(250).IsRequired();
        }
    }
}