using Acme.Domain.Base.Entity;
using System;

namespace Acme.Domain.Base.ValueType
{
    public class Period : ValueObject
    {
        public DateTimeOffset ValidFrom { get; }
        public DateTimeOffset? ValidTill { get; }

        protected Period() { }

        public Period(DateTimeOffset dateFrom, DateTimeOffset? dateTill = null)
        {
            if (dateTill.HasValue && dateTill < dateFrom)
                throw new DomainException("Valid till must be after valid from");

            ValidFrom = dateFrom;
            ValidTill = dateTill;
        }

        public bool IsWithin(DateTimeOffset dateTime) =>
            ValidFrom <= dateTime && (!ValidTill.HasValue || dateTime < ValidTill);

        public bool IsWithin(Period period) => IsWithin(period.ValidFrom, period.ValidTill);

        public bool IsWithin(DateTimeOffset dateFrom, DateTimeOffset? dateTill) =>
            ((!ValidTill.HasValue) || (!ValidTill.HasValue && !dateTill.HasValue)) ||
                ValidFrom <= dateFrom && dateTill <= ValidTill.Value;

        public Period SetValidTill(DateTimeOffset validTill) => new Period(ValidFrom, validTill);
    }
}