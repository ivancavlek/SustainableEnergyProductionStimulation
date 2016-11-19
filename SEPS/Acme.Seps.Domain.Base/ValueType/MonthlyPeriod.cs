using Acme.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Base.ValueType
{
    public class MonthlyPeriod : Period
    {
        protected MonthlyPeriod() { }

        public MonthlyPeriod(DateTimeOffset dateFrom, DateTimeOffset? dateTill = null)
            : base(dateFrom.AddDays(1 - dateFrom.Day).Date,
                  dateTill?.AddDays(1 - dateTill.Value.Day).Date)
        { }

        public override Period SetValidTill(DateTimeOffset validTill) =>
            new MonthlyPeriod(ValidFrom, validTill);
    }
}