using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Base.Entity
{
    public abstract class SepsBaseEntity : BaseEntity
    {
        private readonly ITimeZoneFactory _timeZone;

        public Period Period { get; private set; }

        protected SepsBaseEntity() { }

        protected SepsBaseEntity(ITimeZoneFactory timeZone)
        {
            if (timeZone == null)
                throw new ArgumentNullException(nameof(timeZone));

            _timeZone = timeZone;
            Period = new Period(_timeZone.GetCurrentRepositoryDateTime());
        }

        public void Archive()
        {
            if (IsActive())
                SetExpirationDateTo(_timeZone.GetCurrentRepositoryDateTime());
        }

        public void Delete()
        {
            if (!IsDeleted())
                SetExpirationDateTo(Period.ValidFrom);
        }

        public bool IsActive() => Period.IsWithin(_timeZone.GetCurrentRepositoryDateTime());

        public bool IsDeleted() => Period.ValidTill.HasValue && Period.ValidFrom.Equals(Period.ValidTill);

        public void SetExpirationDateTo(DateTimeOffset expirationDate) =>
            Period = Period.SetValidTill(expirationDate);
    }
}