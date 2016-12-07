using Acme.Seps.Domain.Base.ValueType;
using FluentAssertions;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.ValueType
{
    public class MonthlyPeriodTests
    {
        public void SetsPeriodToFirstOfTheMonthWithoutTime()
        {
            var monthlyPeriod = new MonthlyPeriod(
                new DateTime(2016, 11, 15, 21, 4, 11), new DateTime(2016, 12, 15, 21, 4, 11));

            monthlyPeriod.ValidFrom.Should().Be(new DateTime(2016, 11, 1));
            monthlyPeriod.ValidTill.Should().Be(new DateTime(2016, 12, 1));
        }

        public void DoesNotSetValidTillWhenItIsNotPresent()
        {
            var monthlyPeriod = new MonthlyPeriod(
                new DateTime(2016, 11, 15, 21, 4, 11));

            monthlyPeriod.ValidFrom.Should().Be(new DateTime(2016, 11, 1));
            monthlyPeriod.ValidTill.HasValue.Should().BeFalse();
        }

        public void SetsValidTillToFirstOfTheMonthWithoutTime()
        {
            var monthlyPeriod = new MonthlyPeriod(
                new DateTime(2016, 11, 15, 21, 4, 11));

            var newMonthlyPeriod = monthlyPeriod.SetValidTill(new DateTime(2016, 12, 15, 21, 4, 11));

            newMonthlyPeriod.ValidFrom.Should().Be(new DateTime(2016, 11, 1));
            newMonthlyPeriod.ValidTill.Should().Be(new DateTime(2016, 12, 1));
        }

        public void StringRepresentationShowsCorrectPeriodWhenvalidTillIsNotSet()
        {
            var monthlyPeriod = new MonthlyPeriod(new DateTime(2016, 11, 15, 21, 4, 11));

            monthlyPeriod.ToString().Should().Be($"{monthlyPeriod.ValidFrom:MM.yyyy.} - ");
        }

        public void StringRepresentationShowsCorrectPeriodWhenvalidTillIsSet()
        {
            var monthlyPeriod = new MonthlyPeriod(
                new DateTime(2016, 11, 15, 21, 4, 11), new DateTime(2016, 12, 15, 21, 4, 11));

            monthlyPeriod.ToString().Should().Be(
                $"{monthlyPeriod.ValidFrom:MM.yyyy.} - {monthlyPeriod.ValidTill.Value:MM.yyyy.}");
        }
    }
}