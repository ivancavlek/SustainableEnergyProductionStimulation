using Acme.Seps.Domain.Base.Utility;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Utility;

public class SystemTimeTests
{
    public void CurrentMonthIsCorrectlyExecuted()
    {
        var dateTime = SystemTime.CurrentMonth();

        dateTime.Should().Be(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
    }

    public void CurrentYearIsCorrectlyExecuted()
    {
        var dateTime = SystemTime.CurrentYear();

        dateTime.Should().Be(new DateTime(DateTime.Today.Year, 1, 1));
    }
}