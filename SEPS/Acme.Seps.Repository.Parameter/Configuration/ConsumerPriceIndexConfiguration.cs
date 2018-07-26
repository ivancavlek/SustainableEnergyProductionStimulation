using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    public class ConsumerPriceIndexConfiguration : IEntityTypeConfiguration<ConsumerPriceIndex>
    {
        public void Configure(EntityTypeBuilder<ConsumerPriceIndex> builder)
        {
            builder.HasBaseType<EconometricIndex>();
        }
    }
}