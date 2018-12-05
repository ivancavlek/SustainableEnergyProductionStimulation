using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Base.Entity
{
    public abstract class SepsAggregateRoot : SepsEntity, IAggregateRoot
    {
        public Period Period { get; protected set; }

        protected SepsAggregateRoot() { }

        protected SepsAggregateRoot(DateTimeOffset activeFrom, IIdentityFactory<Guid> identityFactory)
            : base(identityFactory) =>
            Period = new Period(activeFrom);

        public void Deactivate(DateTimeOffset activeTill) =>
            Period = Period.SetValidTill(activeTill);
    }
}