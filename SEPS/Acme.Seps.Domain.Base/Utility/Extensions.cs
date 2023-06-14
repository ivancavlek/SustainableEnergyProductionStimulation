using System;

namespace Acme.Seps.Domain.Base.Utility;

public static class Extensions
{
    public static DateTimeOffset ToFirstDayOfTheMonth(this DateTimeOffset dateTime) =>
        dateTime.AddDays(1 - dateTime.Day).Date;

    public static DateTimeOffset ToFirstDayOfTheYear(this DateTimeOffset dateTime) =>
        dateTime.AddMonths(1 - dateTime.Month).AddDays(1 - dateTime.Day).Date;
}