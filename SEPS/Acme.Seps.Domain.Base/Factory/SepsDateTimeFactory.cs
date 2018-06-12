using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Base.ValueType;
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
            var start = period.ValidFrom.ToFirstDayOfTheMonth();
            var end = period.ValidTill?.ToFirstDayOfTheMonth();

            return new Period(start, end);
        }

        Period ISepsDateTimeFactory.ToYearlyPeriod(Period period)
        {
            var start = period.ValidFrom.ToFirstMonthOfTheYear();
            var end = period.ValidTill?.ToFirstMonthOfTheYear();

            return new Period(start, end);
        }
    }
}