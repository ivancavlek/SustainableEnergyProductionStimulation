using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Text;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class MonthlyEconometricIndexTests
    {
        private readonly decimal _amount;
        private readonly string _remark;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public MonthlyEconometricIndexTests()
        {
            _amount = 1M;
            _remark = nameof(_remark);
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        }

        public void DateCannotBeFromCurrentMonth()
        {
            var currentMonth = DateTime.Now.Date;

            Action action = () => new DummyMonthlyEconometricIndex(
                _amount, _remark, currentMonth, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.MonthlyParameterException);
        }

        public void DateIsCorrectlySet()
        {
            DateTimeOffset monthBeforeCurrentMonth = DateTime.Now.Date.AddMonths(-1);

            var econometricIndex = new DummyMonthlyEconometricIndex(
                _amount, _remark, monthBeforeCurrentMonth, _identityFactory);

            econometricIndex.Active.Since.Should().Be(monthBeforeCurrentMonth.ToFirstDayOfTheMonth());
        }

        public void MonthlyEconometricIndexIsCorrected()
        {
            DateTimeOffset fiveMonthsAgo = DateTime.Now.Date.AddMonths(-5);
            var econometricIndex = new DummyMonthlyEconometricIndex(_amount, _remark, fiveMonthsAgo, _identityFactory);

            const decimal amount = 20M;
            const string remark = "Test";
            DateTimeOffset twoMonthsAgo = DateTime.Now.Date.AddMonths(-2);
            DateTimeOffset nineMonthsAgo = DateTime.Now.Date.AddMonths(-9);
            var previouslyActiveEconometricIndex =
                new DummyMonthlyEconometricIndex(_amount, _remark, nineMonthsAgo, _identityFactory);

            econometricIndex.Correct(
                amount, remark, twoMonthsAgo.Year, twoMonthsAgo.Month, previouslyActiveEconometricIndex);

            previouslyActiveEconometricIndex.Active.Since.Should().Be(nineMonthsAgo.ToFirstDayOfTheMonth());
            previouslyActiveEconometricIndex.Active.Until.Should().Be(twoMonthsAgo.ToFirstDayOfTheMonth());
            econometricIndex.Active.Since.Should().Be(twoMonthsAgo.ToFirstDayOfTheMonth());
            econometricIndex.Active.Until.Should().BeNull();
            econometricIndex.Amount.Should().Be(Math.Round(amount, 2, MidpointRounding.AwayFromZero));
            econometricIndex.Remark.Should().Be(remark);
        }

        private class DummyMonthlyEconometricIndex : MonthlyEconometricIndex<DummyMonthlyEconometricIndex>
        {
            protected override int DecimalPlaces => 2;

            public DummyMonthlyEconometricIndex(
                decimal amount, string remark, DateTimeOffset since, IIdentityFactory<Guid> identityFactory)
                : base(amount, remark, since, identityFactory) { }
        }
    }
}