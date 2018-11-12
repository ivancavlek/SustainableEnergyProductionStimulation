using Acme.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Factory;
using FluentAssertions;
using System;
using Message = Acme.Seps.Domain.Base.Infrastructure.Base;

namespace Acme.Seps.Domain.Base.Test.Unit.Factory
{
    public class YearlyPeriodFactoryTests
    {
        private readonly DateTimeOffset _dateFrom;

        public YearlyPeriodFactoryTests()
        {
            _dateFrom = new DateTime(2017, 5, 13, 13, 15, 11);
        }

        public void YearlyPeriodFactoryIsConfiguredProperly()
        {
            var dateTill = new DateTime(2018, 5, 13, 13, 15, 11);

            var monthlyPeriodFactory = new YearlyPeriodFactory(_dateFrom, dateTill);

            monthlyPeriodFactory.ValidFrom.Should().Be(new DateTimeOffset(new DateTime(2017, 1, 1, 0, 0, 0)));
            monthlyPeriodFactory.ValidTill.Should().Be(new DateTimeOffset(new DateTime(2018, 1, 1, 0, 0, 0)));
        }

        public void DateTillMustBeAfterDateFrom()
        {
            var dateTill = new DateTime(2017, 4, 13, 13, 15, 11);

            Action action = () => new YearlyPeriodFactory(_dateFrom, dateTill);

            action.Should().Throw<DomainException>().WithMessage(Message.ValidTillGreaterThanValidFromException);
        }

        public void DateTillMustBeExactlyOneYearAheadOfDateFrom()
        {
            var dateTill = new DateTime(2019, 4, 13, 13, 15, 11);

            Action action = () => new YearlyPeriodFactory(_dateFrom, dateTill);

            action.Should().Throw<DomainException>().WithMessage(Message.YearlyValueNotEqualYearException);
        }
    }
}