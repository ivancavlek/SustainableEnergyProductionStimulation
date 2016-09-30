using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.ValueType;
using FluentAssertions;
using System;

namespace Acme.Seps.Domain.Base.Test.ValueType
{
    public class PeriodTests
    {
        private readonly DateTimeOffset _dateTimeOffsetUtcNowMinusOneDay;
        private readonly DateTimeOffset _dateTimeOffsetUtcNow;
        private readonly DateTimeOffset _dateTimeOffsetUtcNowPlusOneDay;
        private readonly DateTimeOffset _dateTimeOffsetUtcNowPlusOneMonth;

        public PeriodTests()
        {
            _dateTimeOffsetUtcNow = DateTimeOffset.UtcNow;
            _dateTimeOffsetUtcNowMinusOneDay = _dateTimeOffsetUtcNow.AddDays(-1);
            _dateTimeOffsetUtcNowPlusOneDay = _dateTimeOffsetUtcNow.AddDays(1);
            _dateTimeOffsetUtcNowPlusOneMonth = _dateTimeOffsetUtcNow.AddMonths(1);
        }

        public void CreatesNewPeriodWithDateFrom()
        {
            var result = new Period(_dateTimeOffsetUtcNow);

            result.ValidFrom.Should().Be(_dateTimeOffsetUtcNow);
        }

        public void CreatesNewPeriodWithDateFromAndDateTill()
        {
            var result = new Period(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNowPlusOneMonth);

            result.ValidFrom.Should().Be(_dateTimeOffsetUtcNow);
            result.ValidTill.Should().HaveValue().And.Be(_dateTimeOffsetUtcNowPlusOneMonth);
        }

        public void DoesNotCreateNewPeriodWithDateTillBeforeDateFrom()
        {
            Action action = () => new Period(_dateTimeOffsetUtcNowPlusOneMonth, _dateTimeOffsetUtcNow);

            action.ShouldThrow<DomainException>();
        }

        public void PeriodIsWithinForDateFrom()
        {
            var period = new Period(_dateTimeOffsetUtcNow);

            var result = period.IsWithin(_dateTimeOffsetUtcNowPlusOneDay);

            result.Should().BeTrue();
        }

        public void PeriodIsWithinForDateFromAndDateTill()
        {
            var period = new Period(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNowPlusOneMonth);

            var result = period.IsWithin(_dateTimeOffsetUtcNowPlusOneDay);

            result.Should().BeTrue();
        }

        public void PeriodIsOutsideForDateFromForFaultyDate()
        {
            var period = new Period(_dateTimeOffsetUtcNow);

            var result = period.IsWithin(_dateTimeOffsetUtcNowMinusOneDay);

            result.Should().BeFalse();
        }

        public void PeriodIsOutsideForDateFromAndDateTillForFaultyDate()
        {
            var period = new Period(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNowPlusOneMonth);

            var result = period.IsWithin(_dateTimeOffsetUtcNowMinusOneDay);

            result.Should().BeFalse();
        }

        public void PeriodIsWithinForDateFromAndDateTillWhenDateTillAndValidTillAreNotSet()
        {
            var period = new Period(_dateTimeOffsetUtcNow);

            var result = period.IsWithin(_dateTimeOffsetUtcNowPlusOneDay, null);

            result.Should().BeTrue();
        }

        public void PeriodIsOutsideForDateFromAndDateTillWhenDateTillIsNotSetAndValidTillIsSet()
        {
            var period = new Period(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNowPlusOneMonth);

            var result = period.IsWithin(_dateTimeOffsetUtcNowPlusOneDay, null);

            result.Should().BeFalse();
        }

        public void PeriodIsWithinForDateFromAndDateTillWhenDateTillIsSetAndValidTillIsNotSet()
        {
            var period = new Period(_dateTimeOffsetUtcNow);

            var result = period.IsWithin(_dateTimeOffsetUtcNowPlusOneDay, _dateTimeOffsetUtcNowPlusOneMonth);

            result.Should().BeTrue();
        }

        public void PeriodIsWithinForDateFromAndDateTillWhenDateTillIsSetAndValidTillIsSet()
        {
            var period = new Period(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNowPlusOneMonth);

            var result = period.IsWithin(_dateTimeOffsetUtcNowPlusOneDay, _dateTimeOffsetUtcNowPlusOneDay);

            result.Should().BeTrue();
        }

        public void PeriodIsOutsideForDateFromAndDateTillWhenDateFromIsBeforeValidFrom()
        {
            var period = new Period(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNowPlusOneMonth);

            var result = period.IsWithin(_dateTimeOffsetUtcNowMinusOneDay, _dateTimeOffsetUtcNowPlusOneDay);

            result.Should().BeFalse();
        }

        public void PeriodIsOutsideForDateFromAndDateTillWhenDateTillIsAfterValidTill()
        {
            var period = new Period(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNowPlusOneDay);

            var result = period.IsWithin(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNowPlusOneMonth);

            result.Should().BeFalse();
        }

        public void PeriodIsWithinForDateFromAndDateTillWhenDateFromIsEqualValidFromForCorrectValidTill()
        {
            var period = new Period(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNowPlusOneMonth);

            var result = period.IsWithin(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNowPlusOneDay);

            result.Should().BeTrue();
        }

        public void PeriodIsWithinForDateFromAndDateTillWhenDateTillIsEqualValidTillForCorrectValidFrom()
        {
            var period = new Period(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNowPlusOneMonth);

            var result = period.IsWithin(_dateTimeOffsetUtcNowPlusOneDay, _dateTimeOffsetUtcNowPlusOneMonth);

            result.Should().BeTrue();
        }

        public void CreatesNewPeriodWithValidTill()
        {
            var period = new Period(_dateTimeOffsetUtcNow);

            var result = period.SetValidTill(_dateTimeOffsetUtcNowPlusOneMonth);

            result.ValidFrom.Should().Be(_dateTimeOffsetUtcNow);
            result.ValidTill.Should().HaveValue().And.Be(_dateTimeOffsetUtcNowPlusOneMonth);
        }

        public void EnablesDateFromToBeEqualToDateTill()
        {
            var result = new Period(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNow);

            result.ValidFrom.Should().Be(_dateTimeOffsetUtcNow);
            result.ValidTill.Should().HaveValue().And.Be(_dateTimeOffsetUtcNow);
        }
    }
}