using System;

namespace Acme.Seps.Domain.Base.Utility
{
    public static class Extensions
    {
        public static DateTimeOffset ToFirstDayOfTheMonth(this DateTimeOffset dateTime)
        {
            return dateTime.AddDays(1 - dateTime.Day).Date;
        }

        public static DateTimeOffset ToFirstMonthOfTheYear(this DateTimeOffset dateTime)
        {
            return dateTime.AddMonths(1 - dateTime.Month).AddDays(1 - dateTime.Day).Date;
        }
    }
}