using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Infrastructure;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Base.ValueType;
using Light.GuardClauses;
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

        public void Archive(DateTimeOffset activeTill)
        {
            Period.ActiveTill.HasValue.MustBe(false, (_, __) =>
                new DomainException(SepsBaseMessage.ArchivingArchivedEntityException));

            Period = Period.SetActiveTill(activeTill);
        }

        public bool IsActive() =>
            !Period.ActiveTill.HasValue && Period.ActiveFrom >= SepsVersion.InitialDate();
    }
}