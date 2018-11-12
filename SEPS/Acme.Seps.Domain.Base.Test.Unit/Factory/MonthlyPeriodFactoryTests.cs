using Acme.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Factory;
using FluentAssertions;
using System;
using Message = Acme.Seps.Domain.Base.Infrastructure.Base;

namespace Acme.Seps.Domain.Base.Test.Unit.Factory
{
    public class MonthlyPeriodFactoryTests
    {
        private readonly DateTimeOffset _dateFrom;

        public MonthlyPeriodFactoryTests()
        {
            _dateFrom = new DateTime(2018, 5, 13, 13, 15, 11);
        }

        public void CreatesNewMonthlyPeriodFactoryWithDateFrom()
        {
            var monthlyPeriodFactory = new MonthlyPeriodFactory(_dateFrom);

            monthlyPeriodFactory.ValidFrom.Should().Be(new DateTimeOffset(new DateTime(2018, 5, 1, 0, 0, 0)));
            monthlyPeriodFactory.ValidTill.Should().BeNull();
        }

        public void CreatesNewMonthlyPeriodFactoryWithDateFromAndDateTill()
        {
            var dateTill = new DateTime(2018, 6, 13, 13, 15, 11);

            var monthlyPeriodFactory = new MonthlyPeriodFactory(_dateFrom, dateTill);

            monthlyPeriodFactory.ValidFrom.Should().Be(new DateTimeOffset(new DateTime(2018, 5, 1, 0, 0, 0)));
            monthlyPeriodFactory.ValidTill.Should().Be(new DateTimeOffset(new DateTime(2018, 6, 1, 0, 0, 0)));
        }

        public void DateTillMustBeAfterDateFrom()
        {
            var dateTill = new DateTime(2018, 4, 13, 13, 15, 11);

            Action action = () => new MonthlyPeriodFactory(_dateFrom, dateTill);

            action.Should().Throw<DomainException>().WithMessage(Message.ValidTillGreaterThanValidFromException);
        }
    }
}