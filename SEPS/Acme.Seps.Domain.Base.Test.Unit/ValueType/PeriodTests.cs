using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using FluentAssertions;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.ValueType
{
    public class PeriodTests
    {
        public void PeriodFactoryMustBeSet()
        {
            Action action = () => new Period(null);

            action.Should().Throw<Exception>();
        }

        public void PeriodIsProperlyInitializedWithPeriodFactory()
        {
            var periodFactory = new MonthlyPeriodFactory(DateTime.Now);

            var period = new Period(periodFactory);

            period.ValidFrom.Should().Be(periodFactory.ValidFrom);
            period.ValidTill.Should().Be(periodFactory.ValidTill);
        }
    }
}