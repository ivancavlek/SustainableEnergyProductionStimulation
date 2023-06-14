using Acme.Seps.Domain.Base.Utility;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Utility;

public class ExtensionsTests
{
    private readonly DateTimeOffset _dateTime;

    public ExtensionsTests() => _dateTime = new DateTimeOffset(new DateTime(2012, 5, 14, 18, 44, 12));

    public void ToFirstDayOfTheMonthIsCorrectlyExecuted()
    {
        var result = _dateTime.ToFirstDayOfTheMonth();

        result.Should().Be(new DateTime(2012, 5, 1));
    }

    public void ToFirstMonthOfTheYearIsCorrectlyExecuted()
    {
        var result = _dateTime.ToFirstDayOfTheYear();

        result.Should().Be(new DateTime(2012, 1, 1));
    }
}