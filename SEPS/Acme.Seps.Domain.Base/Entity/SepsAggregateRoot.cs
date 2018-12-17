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
        public ActivePeriod Active { get; private set; }

        protected SepsAggregateRoot() { }

        protected SepsAggregateRoot(DateTimeOffset activeFrom, IIdentityFactory<Guid> identityFactory)
            : base(identityFactory)
        {
            activeFrom.MustBeGreaterThanOrEqualTo(
                SepsVersion.InitialDate(), message: SepsBaseMessage.DateMustBeGreaterThanInitialDate);

            Active = new ActivePeriod(activeFrom);
        }

        protected void SetInactive(DateTimeOffset inactiveFrom)
        {
            Active.Until.HasValue.MustBe(false, (_, __) =>
                new DomainException(SepsBaseMessage.ArchivingArchivedEntityException));

            Active = Active.SetActiveUntil(inactiveFrom);
        }

        protected void CorrectActiveSince(DateTimeOffset newActiveSince)
        {
            Active = new ActivePeriod(newActiveSince);
        }

        protected void CorrectActiveUntil(DateTimeOffset newActiveUntil)
        {
            Active = Active.SetActiveUntil(newActiveUntil);
        }

        public bool IsActive() =>
            !Active.Until.HasValue;
    }
}