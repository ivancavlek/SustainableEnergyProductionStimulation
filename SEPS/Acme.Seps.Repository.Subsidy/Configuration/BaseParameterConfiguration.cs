﻿using Acme.Seps.Domain.Base.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.Seps.Repository.Subsidy.Configuration;

internal class BaseParameterConfiguration<TParameterEntity> where TParameterEntity : SepsAggregateRoot
{
    public virtual void Configure(EntityTypeBuilder<TParameterEntity> builder) =>
        //builder.Property<byte[]>("RowVersion").IsRowVersion();
        ConfigureValueTypes(builder);

    private static void ConfigureValueTypes(EntityTypeBuilder<TParameterEntity> builder) =>
        builder.OwnsOne(vte => vte.Active, vte =>
        {
            vte.Property(ppy => ppy.Since).HasColumnName("Since").IsRequired();
            vte.Property(ppy => ppy.Until).HasColumnName("Until");
        });
}