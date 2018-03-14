using Acme.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Base.Factory
{
    public sealed class SepsDateTimeFactory : ISepsDateTimeFactory
    {
        private readonly DateTimeOffset _currentDateTime;

        public SepsDateTimeFactory(DateTimeOffset currentDateTime) =>
            _currentDateTime = currentDateTime;

        DateTime ISepsDateTimeFactory.CurrentDate =>
            _currentDateTime.Date;

        int ISepsDateTimeFactory.CurrentYear =>
            _currentDateTime.Year;

        Period ISepsDateTimeFactory.ToMonthlyPeriod(Period period)
        {
            var start = period.ValidFrom.AddDays(1 - period.ValidFrom.Day).DateTime;
            var end = period.ValidTill?.AddDays(1 - period.ValidTill.Value.Day).DateTime;

            return new Period(start, end);
        }

        Period ISepsDateTimeFactory.ToYearlyPeriod(Period period)
        {
            var start = period.ValidFrom.AddMonths(1 - period.ValidFrom.Month).DateTime;
            var end = period.ValidTill?.AddMonths(1 - period.ValidTill.Value.Month).DateTime;

            return new Period(start, end);
        }
    }
}