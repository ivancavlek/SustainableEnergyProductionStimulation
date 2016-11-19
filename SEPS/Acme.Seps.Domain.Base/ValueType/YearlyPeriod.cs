using Acme.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Base.ValueType
{
    public class YearlyPeriod : Period
    {
        protected YearlyPeriod() { }

        public YearlyPeriod(DateTimeOffset dateFrom, DateTimeOffset? dateTill = null)
            : base(dateFrom.AddMonths(1 - dateFrom.Month).AddDays(1 - dateFrom.Day).Date,
                  dateTill?.AddMonths(1 - dateTill.Value.Month).AddDays(1 - dateTill.Value.Day).Date)
        { }

        public override Period SetValidTill(DateTimeOffset validTill) =>
            new YearlyPeriod(ValidFrom, validTill);
    }
}