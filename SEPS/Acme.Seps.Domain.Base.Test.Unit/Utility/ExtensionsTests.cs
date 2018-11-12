using Acme.Seps.Domain.Base.Utility;
using FluentAssertions;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Utility
{
    public class ExtensionsTests
    {
        public void ToFirstDayOfTheMonthIsCorrectlyExecuted()
        {
            var dateTime = new DateTimeOffset(new DateTime(2012, 5, 14, 18, 44, 12));

            var result = dateTime.ToFirstDayOfTheMonth();

            result.Should().Be(new DateTime(2012, 5, 1));
        }

        public void ToFirstMonthOfTheYearIsCorrectlyExecuted()
        {
            var dateTime = new DateTimeOffset(new DateTime(2012, 5, 14, 18, 44, 12));

            var result = dateTime.ToFirstMonthOfTheYear();

            result.Should().Be(new DateTime(2012, 1, 1));
        }
    }
}