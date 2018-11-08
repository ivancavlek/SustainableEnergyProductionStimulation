using Acme.Seps.Domain.Base.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    public class BaseParameterConfiguration<TParameterEntity> where TParameterEntity : SepsBaseAggregate
    {
        public virtual void Configure(EntityTypeBuilder<TParameterEntity> builder)
        {
            builder.Property<byte[]>("RowVersion").IsRowVersion();
        }
    }
}