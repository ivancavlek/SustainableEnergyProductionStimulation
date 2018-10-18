using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    public class TariffConfiguration : BaseParameterConfiguration<Tariff>, IEntityTypeConfiguration<Tariff>
    {
        private readonly string _discriminator;

        public TariffConfiguration()
        {
            _discriminator = "TariffType";
        }

        public override void Configure(EntityTypeBuilder<Tariff> builder)
        {
            builder.Property<string>(_discriminator).HasMaxLength(50);

            builder
                .ToTable("Tariffs")
                .HasDiscriminator<string>(_discriminator)
                .HasValue<CogenerationTariff>(nameof(CogenerationTariff))
                .HasValue<RenewableEnergySourceTariff>(nameof(RenewableEnergySourceTariff));

            base.Configure(builder);
        }
    }
}