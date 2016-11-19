using Acme.Seps.Domain.Base.ValueType;
using FluentAssertions;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.ValueType
{
    public class YearlyPeriodTests
    {
        public void SetsPeriodToFirstOfTheMonthAndFirstOfTheYearWithoutTime()
        {
            var yearlyPeriod = new YearlyPeriod(
                new DateTime(2016, 11, 15, 21, 4, 11), new DateTime(2017, 12, 15, 21, 4, 11));

            yearlyPeriod.ValidFrom.Should().Be(new DateTime(2016, 1, 1));
            yearlyPeriod.ValidTill.Should().Be(new DateTime(2017, 1, 1));
        }

        public void DoesNotSetValidTillWhenItIsNotPresent()
        {
            var yearlyPeriod = new YearlyPeriod(
                new DateTime(2016, 11, 15, 21, 4, 11));

            yearlyPeriod.ValidFrom.Should().Be(new DateTime(2016, 1, 1));
            yearlyPeriod.ValidTill.HasValue.Should().BeFalse();
        }

        public void SetsValidTillToFirstOfTheMonthAndFirstOfTheYearWithoutTime()
        {
            var yearlyPeriod = new YearlyPeriod(
                new DateTime(2016, 11, 15, 21, 4, 11));

            var newYearlyPeriod = yearlyPeriod.SetValidTill(new DateTime(2017, 12, 15, 21, 4, 11));

            newYearlyPeriod.ValidFrom.Should().Be(new DateTime(2016, 1, 1));
            newYearlyPeriod.ValidTill.Should().Be(new DateTime(2017, 1, 1));
        }
    }
}