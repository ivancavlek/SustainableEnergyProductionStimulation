using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    internal sealed class ConsumerPriceIndexConfiguration
        : BaseParameterConfiguration<ConsumerPriceIndex>, IEntityTypeConfiguration<ConsumerPriceIndex>
    {
        private readonly Guid _id;

        internal ConsumerPriceIndexConfiguration(Guid id)
        {
            _id = id;
        }

        public override void Configure(EntityTypeBuilder<ConsumerPriceIndex> builder)
        {
            base.Configure(builder);

            builder.HasData(
                new
                {
                    Id = _id,
                    Amount = 1M,
                    Remark = "Initial value",
                    EconometricIndexType = nameof(ConsumerPriceIndex)
                });
            builder.OwnsOne(vte => vte.Period, vte =>
            {
                vte.HasData(new
                {
                    EconometricIndexId = _id,
                    ValidFrom = new DateTimeOffset(new DateTime(2016, 01, 01)),
                    ValidTill = new DateTimeOffset(new DateTime(2017, 01, 01))
                });
            });
        }
    }
}