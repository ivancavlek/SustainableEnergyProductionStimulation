using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    public class ConsumerPriceIndexConfiguration
        : BaseParameterConfiguration<ConsumerPriceIndex>, IEntityTypeConfiguration<ConsumerPriceIndex>
    {
        public override void Configure(EntityTypeBuilder<ConsumerPriceIndex> builder)
        {
            base.Configure(builder);

            var guid = Guid.NewGuid();

            builder.HasData(
                new
                {
                    Id = guid,
                    Amount = 1M,
                    Remark = "Initial value",
                    EconometricIndexType = nameof(ConsumerPriceIndex)
                });
            builder.OwnsOne(vte => vte.Period, vte =>
            {
                vte.HasData(new
                {
                    EconometricIndexId = guid,
                    ValidFrom = new DateTimeOffset(new DateTime(2016, 01, 01)),
                    ValidTill = new DateTimeOffset(new DateTime(2017, 01, 01))
                });
            });
        }
    }
}