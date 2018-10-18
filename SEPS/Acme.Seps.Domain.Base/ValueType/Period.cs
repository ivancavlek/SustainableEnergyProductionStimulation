using Acme.Domain.Base.Entity;
using Acme.Domain.Base.ValueType;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Base.Infrastructure.Base;

namespace Acme.Seps.Domain.Base.ValueType
{
    public class Period : ValueObject
    {
        public DateTimeOffset ValidFrom { get; private set; }
        public DateTimeOffset? ValidTill { get; private set; }

        protected Period()
        {
        }

        public Period(DateTimeOffset dateFrom)
            : this(dateFrom, null) { }

        public Period(DateTimeOffset dateFrom, DateTimeOffset? dateTill)
        {
            if (dateTill.HasValue)
                dateTill.Value.MustBeGreaterThanOrEqualTo(dateFrom, (x, y) => new DomainException(Message.ValidTillGreaterThanValidFromException));

            ValidFrom = dateFrom;
            ValidTill = dateTill;
        }

        public virtual Period SetValidTill(DateTimeOffset validTill) =>
            new Period(ValidFrom, validTill);
    }
}