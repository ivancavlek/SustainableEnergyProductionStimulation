using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Text;
using System;

namespace Acme.Seps.Domain.Base.Entity;

public abstract class SepsAggregateRoot : SepsEntity, IAggregateRoot
{
    private readonly ActiveSpecification<SepsAggregateRoot> _isActive = new();

    public ActivePeriod Active { get; private set; }

    protected SepsAggregateRoot() { }

    protected SepsAggregateRoot(DateTimeOffset since, IIdentityFactory<Guid> identityFactory)
        : base(identityFactory)
    {
        since.MustBeGreaterThanOrEqualTo(
            SepsVersion.InitialDate(),
            message: SepsMessage.ValueHigherThanTheOther(since.Date.ToShortDateString(), SepsVersion.InitialDate().Date.ToShortDateString()));

        Active = new ActivePeriod(since);
    }

    protected void SetInactive(DateTimeOffset inactiveFrom)
    {
        _isActive.IsSatisfiedBy(this).MustBe(true, (_, __) =>
            new DomainException(SepsMessage.CannotDeactivateInactiveEntity(GetType().Name)));

        Active = Active.SetActiveUntil(inactiveFrom);
    }

    protected void CorrectActiveSince(DateTimeOffset newActiveSince) =>
        Active = new ActivePeriod(newActiveSince);

    protected void CorrectActiveUntil(DateTimeOffset newActiveUntil) =>
        Active = Active.SetActiveUntil(newActiveUntil);

    public bool IsActive() =>
        _isActive.IsSatisfiedBy(this);
}