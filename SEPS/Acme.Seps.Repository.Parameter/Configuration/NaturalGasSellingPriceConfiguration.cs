using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    public class NaturalGasSellingPriceConfiguration : IEntityTypeConfiguration<NaturalGasSellingPrice>
    {
        public void Configure(EntityTypeBuilder<NaturalGasSellingPrice> builder)
        {
            var guid = Guid.NewGuid();

            builder.HasData(
                new
                {
                    Id = guid,
                    Amount = 4M,
                    Remark = "Initial value",
                    EconometricIndexType = nameof(NaturalGasSellingPrice)
                });
            builder.OwnsOne(vte => vte.Period, vte =>
            {
                vte.HasData(new
                {
                    EconometricIndexId = guid,
                    ValidFrom = new DateTimeOffset(new DateTime(2007, 07, 01)),
                });
            });
        }
    }
}