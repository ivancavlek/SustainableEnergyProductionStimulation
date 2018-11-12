using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Base.Entity
{
    public abstract class SepsBaseAggregate : SepsBaseEntity, IAggregateRoot
    {
        public Period Period { get; protected set; }

        protected SepsBaseAggregate() { }

        protected SepsBaseAggregate(IPeriodFactory periodFactory, IIdentityFactory<Guid> identityFactory)
            : base(identityFactory)
        {
            periodFactory.MustNotBeNull(nameof(periodFactory));

            Period = new Period(periodFactory);
        }
    }
}