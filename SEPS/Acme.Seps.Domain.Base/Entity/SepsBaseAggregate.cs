using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Base.Infrastructure.Base;

namespace Acme.Seps.Domain.Base.Entity
{
    public abstract class SepsBaseAggregate : SepsBaseEntity, IAggregateRoot
    {
        public Period Period { get; private set; }

        protected SepsBaseAggregate()
        {
        }

        protected SepsBaseAggregate(Period period, IIdentityFactory<Guid> identityFactory)
            : base(identityFactory)
        {
            period.ValidFrom.Day.MustBe(1, (x, y) => new DomainException(Message.DailyEconometricIndexNotAllowedException));
            period.ValidFrom.TimeOfDay.MustBe(TimeSpan.Zero, (x, y) => new DomainException(Message.TimeOfDayPeriodNotAllowedException));
            if (period.ValidTill.HasValue)
            {
                period.ValidTill.Value.Day.MustBe(1, (x, y) => new DomainException(Message.DailyEconometricIndexNotAllowedException));
                period.ValidTill.Value.TimeOfDay.MustBe(TimeSpan.Zero, (x, y) => new DomainException(Message.TimeOfDayPeriodNotAllowedException));
            }

            Period = period;
        }

        public void SetExpirationDateTo(DateTimeOffset expirationDate) =>
            Period = Period.SetValidTill(expirationDate);
    }
}