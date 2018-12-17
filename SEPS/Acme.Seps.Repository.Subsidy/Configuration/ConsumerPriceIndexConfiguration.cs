using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.UseCases.Subsidy.Command.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Acme.Seps.Repository.Subsidy.Configuration
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
            SeedData(builder);
        }

        private void SeedData(EntityTypeBuilder<ConsumerPriceIndex> builder)
        {
            builder.HasData(
                new
                {
                    Id = _id,
                    Amount = 105.8M,
                    Remark = "Initial value",
                    EconometricIndexType = nameof(ConsumerPriceIndex)
                });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    EconometricIndexId = _id,
                    Since = SepsVersion.InitialDate()
                });
            });
        }
    }
}