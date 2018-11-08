using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    public class EconometricIndexConfiguration
        : BaseParameterConfiguration<EconometricIndex>, IEntityTypeConfiguration<EconometricIndex>
    {
        private readonly string _discriminator;

        public EconometricIndexConfiguration()
        {
            _discriminator = "EconometricIndexType";
        }

        public override void Configure(EntityTypeBuilder<EconometricIndex> builder)
        {
            builder.Property<string>(_discriminator).HasMaxLength(50);
            builder.Property(ppy => ppy.Remark).HasMaxLength(250).IsRequired();

            builder
                .ToTable("EconometricIndexes")
                .HasDiscriminator<string>(_discriminator)
                .HasValue<ConsumerPriceIndex>(nameof(ConsumerPriceIndex))
                .HasValue<NaturalGasSellingPrice>(nameof(NaturalGasSellingPrice))
                .HasValue<MonthlyAverageElectricEnergyProductionPrice>(nameof(MonthlyAverageElectricEnergyProductionPrice));

            base.Configure(builder);
        }
    }
}