using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Base.Entity
{
    public abstract class SepsBaseEntity : Entity<Guid>
    {
        public Period Period { get; protected set; }

        protected SepsBaseEntity() { }

        protected SepsBaseEntity(Period period, IIdentityFactory<Guid> identityFactory)
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

        public bool IsActiveAt(DateTimeOffset dateTime) => Period.IsWithin(dateTime);

        public void SetExpirationDateTo(DateTimeOffset expirationDate) =>
            Period = Period.SetValidTill(expirationDate);
    }
}