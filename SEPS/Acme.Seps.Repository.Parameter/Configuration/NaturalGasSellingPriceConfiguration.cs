﻿using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    public class NaturalGasSellingPriceConfiguration : IEntityTypeConfiguration<NaturalGasSellingPrice>
    {
        public void Configure(EntityTypeBuilder<NaturalGasSellingPrice> builder)
        {
            builder.HasBaseType<EconometricIndex>();
        }
    }
}