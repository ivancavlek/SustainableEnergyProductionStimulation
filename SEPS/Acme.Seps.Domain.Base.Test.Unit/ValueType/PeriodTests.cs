using Acme.Domain.Base.Entity;
using Acme.Seps.Domain.Base.ValueType;
using FluentAssertions;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.ValueType
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

        //public void CreatesNewPeriodWithDateFrom()
        //{
        //    var result = new Period(_dateTimeOffsetUtcNow);

        //    result.ValidFrom.Should().Be(_dateTimeOffsetUtcNow);
        //}

        //public void CreatesNewPeriodWithDateFromAndDateTill()
        //{
        //    var result = new Period(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNowPlusOneMonth);

        //    result.ValidFrom.Should().Be(_dateTimeOffsetUtcNow);
        //    result.ValidTill.Should().HaveValue().And.Be(_dateTimeOffsetUtcNowPlusOneMonth);
        //}

        //public void DoesNotCreateNewPeriodWithDateTillBeforeDateFrom()
        //{
        //    Action action = () => new Period(_dateTimeOffsetUtcNowPlusOneMonth, _dateTimeOffsetUtcNow);

        //    action.Should().Throw<DomainException>();
        //}

        //public void CreatesNewPeriodWithValidTill()
        //{
        //    var period = new Period(_dateTimeOffsetUtcNow);

        //    var result = period.SetValidTill(_dateTimeOffsetUtcNowPlusOneMonth);

        //    result.ValidFrom.Should().Be(_dateTimeOffsetUtcNow);
        //    result.ValidTill.Should().HaveValue().And.Be(_dateTimeOffsetUtcNowPlusOneMonth);
        //}

        //public void EnablesDateFromToBeEqualToDateTill()
        //{
        //    var result = new Period(_dateTimeOffsetUtcNow, _dateTimeOffsetUtcNow);

        //    result.ValidFrom.Should().Be(_dateTimeOffsetUtcNow);
        //    result.ValidTill.Should().HaveValue().And.Be(_dateTimeOffsetUtcNow);
        //}
    }
}