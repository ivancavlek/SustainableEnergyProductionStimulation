using Acme.Domain.Base.Entity;
using Acme.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Base.ValueType
{
    public class Period : ValueObject
    {
        public DateTimeOffset ValidFrom { get; }
        public DateTimeOffset? ValidTill { get; }

        protected Period()
        {
        }

        public Period(DateTimeOffset dateFrom, DateTimeOffset? dateTill = null)
        {
            if (dateTill.HasValue && dateTill < dateFrom)
                throw new DomainException("Valid till must be after valid from");

            ValidFrom = dateFrom;
            ValidTill = dateTill;
        }

        public virtual Period SetValidTill(DateTimeOffset validTill) =>
            new Period(ValidFrom, validTill);
    }
}