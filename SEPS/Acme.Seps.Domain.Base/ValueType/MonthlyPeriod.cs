using Acme.Seps.Domain.Base.Utility;
using System;

namespace Acme.Seps.Domain.Base.ValueType
{
    public class MonthlyPeriod : Period
    {
        protected MonthlyPeriod()
        {
        }

        public MonthlyPeriod(DateTimeOffset dateFrom)
            : base(dateFrom.ToFirstDayOfTheMonth(), null) { }

        public MonthlyPeriod(DateTimeOffset dateFrom, DateTimeOffset dateTill)
            : base(dateFrom.ToFirstDayOfTheMonth(), dateTill.ToFirstDayOfTheMonth()) { }

        public override Period SetValidTill(DateTimeOffset validTill) =>
            new MonthlyPeriod(ValidFrom, validTill);

        public override string ToString() =>
            ValidTill.HasValue ? $"{ValidFrom:MM.yyyy.} - {ValidTill.Value:MM.yyyy.}" : $"{ValidFrom:MM.yyyy.} - ";
    }
}