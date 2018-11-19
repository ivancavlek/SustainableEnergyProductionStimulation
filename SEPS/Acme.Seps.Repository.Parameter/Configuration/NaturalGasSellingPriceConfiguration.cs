using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    internal sealed class NaturalGasSellingPriceConfiguration
        : BaseParameterConfiguration<NaturalGasSellingPrice>,
        IEntityTypeConfiguration<NaturalGasSellingPrice>
    {
        private readonly Guid _id;

        internal NaturalGasSellingPriceConfiguration(Guid id)
        {
            _id = id;
        }

        public override void Configure(EntityTypeBuilder<NaturalGasSellingPrice> builder)
        {
            base.Configure(builder);

            builder.HasData(
                new
                {
                    Id = _id,
                    Amount = 4M,
                    Remark = "Initial value",
                    EconometricIndexType = nameof(NaturalGasSellingPrice)
                });
            builder.OwnsOne(vte => vte.Period, vte =>
            {
                vte.HasData(new
                {
                    EconometricIndexId = _id,
                    ValidFrom = new DateTimeOffset(new DateTime(2007, 07, 01)),
                });
            });
        }
    }
}