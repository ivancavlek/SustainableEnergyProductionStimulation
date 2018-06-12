using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Base.Entity
{
    public abstract class SepsBaseAggregate : SepsBaseEntity, IAggregateRoot
    {
        public Period Period { get; protected set; }

        protected SepsBaseAggregate()
        {
        }

        protected SepsBaseAggregate(Period period, IIdentityFactory<Guid> identityFactory)
            : base(identityFactory)
        {
            if (!period.ValidFrom.Day.Equals(1) ||
                (period.ValidTill.HasValue && !period.ValidTill.Value.Day.Equals(1)))
                throw new DomainException(Infrastructure.Base.DailyEconometricIndexNotAllowedException);
            if (!period.ValidFrom.TimeOfDay.Equals(TimeSpan.Zero) ||
                (period.ValidTill.HasValue && !period.ValidTill.Value.TimeOfDay.Equals(TimeSpan.Zero)))
                throw new DomainException(Infrastructure.Base.TimeOfDayPeriodNotAllowedException);

            Period = period;
        }

        public bool IsActiveAt(DateTimeOffset dateTime) =>
            new ActiveAtDateSpecification<SepsBaseAggregate>(dateTime).IsSatisfiedBy(this);

        public bool IsWithin(Period period) =>
            IsWithin(period.ValidFrom, period.ValidTill);

        public bool IsWithin(DateTimeOffset dateFrom, DateTimeOffset? dateTill) =>
            new ActiveWithinDatesSpecification<SepsBaseAggregate>(dateFrom, dateTill).IsSatisfiedBy(this);

        public void SetExpirationDateTo(DateTimeOffset expirationDate) =>
            Period = Period.SetValidTill(expirationDate);
    }
}