using Acme.Domain.Base.DomainService;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Base.Entity
{
    public abstract class SepsBaseEntity : BaseEntity
    {
        private readonly ITimeZone _timeZone;

        public Period Period { get; private set; }

        protected SepsBaseEntity(ITimeZone timeZone)
        {
            if (timeZone == null)
                throw new ArgumentNullException(nameof(timeZone));

            _timeZone = timeZone;
            Period = new Period(_timeZone.GetCurrentRepositoryDateTime());
        }

        public void Archive()
        {
            if (!IsArchived())
                Period = Period.SetValidTill(_timeZone.GetCurrentRepositoryDateTime());
        }

        private bool IsArchived() => Period.ValidTill.HasValue;
    }
}