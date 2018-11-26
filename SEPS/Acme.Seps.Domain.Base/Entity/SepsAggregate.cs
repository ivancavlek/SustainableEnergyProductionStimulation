using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Base.Entity
{
    public abstract class SepsAggregate : SepsEntity, IAggregateRoot
    {
        public Period Period { get; protected set; }

        protected SepsAggregate() { }

        protected SepsAggregate(IPeriodFactory periodFactory, IIdentityFactory<Guid> identityFactory)
            : base(identityFactory)
        {
            periodFactory.MustNotBeNull(nameof(periodFactory));

            Period = new Period(periodFactory);
        }
    }
}