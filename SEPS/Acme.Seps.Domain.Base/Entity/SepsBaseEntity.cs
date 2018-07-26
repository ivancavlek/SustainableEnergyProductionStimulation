using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Base.Entity
{
    public abstract class SepsBaseEntity : Entity<Guid>
    {
        public byte[] RowVersion { get; set; }

        protected SepsBaseEntity()
        {
        }

        protected SepsBaseEntity(IIdentityFactory<Guid> identityFactory)
            : base(identityFactory) { }
    }
}