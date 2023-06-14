using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Light.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Acme.Seps.Repository.Subsidy.Configuration;

internal sealed class NaturalGasSellingPriceConfiguration
    : BaseParameterConfiguration<NaturalGasSellingPrice>,
    IEntityTypeConfiguration<NaturalGasSellingPrice>
{
    private readonly Guid _id;

    internal NaturalGasSellingPriceConfiguration(Guid id) =>
        _id = id.MustNotBeDefault(nameof(id));

    public override void Configure(EntityTypeBuilder<NaturalGasSellingPrice> builder)
    {
        base.Configure(builder);
        SeedData(builder);
    }

    private void SeedData(EntityTypeBuilder<NaturalGasSellingPrice> builder)
    {
        builder.HasData(
            new
            {
                Id = _id,
                Amount = 1.07M,
                Remark = "Initial value",
                EconometricIndexType = nameof(NaturalGasSellingPrice)
            });
        builder.OwnsOne(vte => vte.Active, vte =>
            vte.HasData(new
            {
                EconometricIndexId = _id,
                Since = SepsVersion.InitialDate(),
            }));
    }
}