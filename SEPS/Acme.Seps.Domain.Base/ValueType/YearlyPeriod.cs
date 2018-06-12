using Acme.Seps.Domain.Base.Utility;
using System;

namespace Acme.Seps.Domain.Base.ValueType
{
    public class YearlyPeriod : Period
    {
        protected YearlyPeriod()
        {
        }

        public YearlyPeriod(DateTimeOffset dateFrom)
            : base(dateFrom.ToFirstMonthOfTheYear().ToFirstDayOfTheMonth(), null) { }

        public YearlyPeriod(DateTimeOffset dateFrom, DateTimeOffset dateTill)
            : base(dateFrom.ToFirstMonthOfTheYear().ToFirstDayOfTheMonth(),
                  dateTill.ToFirstMonthOfTheYear().ToFirstDayOfTheMonth())
        { }

        public override Period SetValidTill(DateTimeOffset validTill) =>
            new YearlyPeriod(ValidFrom, validTill);

        public override string ToString() =>
            $"{ValidFrom:yyyy}";
    }
}