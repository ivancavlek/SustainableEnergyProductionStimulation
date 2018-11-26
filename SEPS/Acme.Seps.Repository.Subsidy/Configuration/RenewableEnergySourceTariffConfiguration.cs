using Acme.Seps.Domain.Subsidy.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Acme.Seps.Repository.Subsidy.Configuration
{
    internal sealed class RenewableEnergySourceTariffConfiguration
        : BaseParameterConfiguration<RenewableEnergySourceTariff>,
        IEntityTypeConfiguration<RenewableEnergySourceTariff>
    {
        private readonly Guid _cpiId;

        internal RenewableEnergySourceTariffConfiguration(Guid cpiId)
        {
            _cpiId = cpiId;
        }

        public override void Configure(EntityTypeBuilder<RenewableEnergySourceTariff> builder)
        {
            base.Configure(builder);

            var id = Guid.NewGuid();

            builder.HasData(new
            {
                Id = id,
                LowerProductionLimit = 100,
                UpperProductionLimit = 200,
                LowerRate = 10M,
                HigherRate = 20M,
                TarrifType = nameof(RenewableEnergySourceTariff),
                ConsumerPriceIndexId = _cpiId
            });
            builder.OwnsOne(vte => vte.Period, vte =>
            {
                vte.HasData(new
                {
                    TariffId = id,
                    ValidFrom = new DateTimeOffset(new DateTime(2016, 01, 01)),
                    ValidTill = new DateTimeOffset(new DateTime(2017, 01, 01))
                });
            });
        }
    }
}