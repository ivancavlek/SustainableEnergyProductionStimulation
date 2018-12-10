using Acme.Seps.Domain.Subsidy.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.Seps.Repository.Subsidy.Configuration
{
    internal sealed class TariffConfiguration : BaseParameterConfiguration<Tariff>, IEntityTypeConfiguration<Tariff>
    {
        private readonly string _discriminator;

        internal TariffConfiguration()
        {
            _discriminator = "TariffType";
        }

        public override void Configure(EntityTypeBuilder<Tariff> builder)
        {
            ConfigureProperties(builder);
            ConfigureTables(builder);

            base.Configure(builder);
        }

        private void ConfigureProperties(EntityTypeBuilder<Tariff> builder)
        {
            builder.Property<string>(_discriminator).HasMaxLength(50);
        }

        private void ConfigureTables(EntityTypeBuilder<Tariff> builder)
        {
            builder
                .ToTable("Tariffs")
                .HasDiscriminator<string>(_discriminator)
                .HasValue<CogenerationTariff>(nameof(CogenerationTariff))
                .HasValue<RenewableEnergySourceTariff>(nameof(RenewableEnergySourceTariff));
        }
    }
}