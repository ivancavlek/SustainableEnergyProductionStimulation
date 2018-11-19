using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    internal sealed class CogenerationTariffConfiguration
        : BaseParameterConfiguration<CogenerationTariff>, IEntityTypeConfiguration<CogenerationTariff>
    {
        private readonly Guid _naturalGasSellingPriceId;

        internal CogenerationTariffConfiguration(Guid naturalGasSellingPriceId)
        {
            _naturalGasSellingPriceId = naturalGasSellingPriceId;
        }

        public override void Configure(EntityTypeBuilder<CogenerationTariff> builder)
        {
            base.Configure(builder);

            var id = Guid.NewGuid();

            builder.HasData(new
            {
                Id = id,
                LowerProductionLimit = 500,
                UpperProductionLimit = 1000,
                LowerRate = 250M,
                HigherRate = 750M,
                TarrifType = nameof(CogenerationTariff),
                NaturalGasSellingPriceId = _naturalGasSellingPriceId
            });
            builder.OwnsOne(vte => vte.Period, vte =>
            {
                vte.HasData(new
                {
                    TariffId = id,
                    ValidFrom = new DateTimeOffset(new DateTime(2007, 07, 01))
                });
            });
        }
    }
}