using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Base.Entity
{
    public abstract class SepsBaseAggregate : SepsBaseEntity, IAggregateRoot
    {
        protected SepsBaseAggregate()
        {
        }

        protected SepsBaseAggregate(IIdentityFactory<Guid> identityFactory)
            : base(identityFactory) { }
    }
}