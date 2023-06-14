using System;

namespace Acme.Seps.Domain.Base.Utility;

public static class SystemTime
{
    public static Func<DateTimeOffset> CurrentMonth = () =>
        DateTime.Today.AddDays(1 - DateTime.Today.Day).Date;

    public static Func<DateTimeOffset> CurrentYear = () =>
        DateTime.Today.AddMonths(1 - DateTime.Today.Month).AddDays(1 - DateTime.Today.Day).Date;
}