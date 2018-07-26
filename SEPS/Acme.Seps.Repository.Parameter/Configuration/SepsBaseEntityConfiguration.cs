using Acme.Seps.Domain.Base.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    public class SepsBaseEntityConfiguration : IEntityTypeConfiguration<SepsBaseEntity>
    {
        public void Configure(EntityTypeBuilder<SepsBaseEntity> builder)
        {
            builder.Property(ppy => ppy.RowVersion).IsRowVersion();
        }
    }
}