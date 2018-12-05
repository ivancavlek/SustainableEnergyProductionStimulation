using Acme.Domain.Base.Entity;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Infrastructure;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Base.ValueType
{
    public sealed class Period : ValueObject
    {
        public DateTimeOffset ValidFrom { get; private set; }
        public DateTimeOffset? ValidTill { get; private set; }

        private Period() { }

        private Period(DateTimeOffset validFrom, DateTimeOffset dateTill)
        {
            validFrom.MustBeGreaterThanOrEqualTo(dateTill, (_, __) =>
                new DomainException(SepsBaseMessage.ValidTillGreaterThanValidFromException));

            ValidFrom = validFrom;
            ValidTill = dateTill;
        }

        internal Period(DateTimeOffset validFrom) =>
            ValidFrom = validFrom;

        internal Period SetValidTill(DateTimeOffset validTill) =>
            new Period(ValidFrom, validTill);

        public override string ToString() =>
            string.Concat(
                ValidFrom.Date.ToShortDateString(),
                " - ",
                ValidTill.HasValue ? ValidTill.Value.Date.ToShortDateString() : "##.##.####");
    }
}