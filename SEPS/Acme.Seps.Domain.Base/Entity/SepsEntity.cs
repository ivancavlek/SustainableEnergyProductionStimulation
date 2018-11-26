using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Base.Entity
{
    public abstract class SepsEntity : Entity<Guid>
    {
        protected SepsEntity() { }

        protected SepsEntity(IIdentityFactory<Guid> identityFactory)
            : base(identityFactory) { }
    }
}