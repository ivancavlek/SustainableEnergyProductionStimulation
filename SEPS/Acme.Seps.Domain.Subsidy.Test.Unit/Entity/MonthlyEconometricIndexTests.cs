using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class MonthlyEconometricIndexTests
    {
        private readonly decimal _amount;
        private readonly int _decimalPlaces;
        private readonly string _remark;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public MonthlyEconometricIndexTests()
        {
            _amount = 1M;
            _decimalPlaces = 2;
            _remark = nameof(_remark);
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        }

        public void DateCannotBeBeforeInitialDate()
        {
            var monthBeforeInitialDate = SepsVersion.InitialDate().AddMonths(-1);

            Action action = () => new DummyMonthlyEconometricIndex(
                _amount, _decimalPlaces, _remark, monthBeforeInitialDate, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.MonthlyParameterException);
        }

        public void DateCannotBeFromCurrentMonth()
        {
            var currentMonth = DateTime.Now.Date;

            Action action = () => new DummyMonthlyEconometricIndex(
                _amount, _decimalPlaces, _remark, currentMonth, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.MonthlyParameterException);
        }

        public void DateIsCorrectlySet()
        {
            DateTimeOffset monthBeforeCurrentMonth = DateTime.Now.Date.AddMonths(-1);

            var econometricIndex = new DummyMonthlyEconometricIndex(
                _amount, _decimalPlaces, _remark, monthBeforeCurrentMonth, _identityFactory);

            econometricIndex.Period.ActiveFrom.Should().Be(monthBeforeCurrentMonth.ToFirstDayOfTheMonth());
        }

        private class DummyMonthlyEconometricIndex : MonthlyEconometricIndex<DummyMonthlyEconometricIndex>
        {
            public DummyMonthlyEconometricIndex(
                decimal amount,
                int decimalPlaces,
                string remark,
                DateTimeOffset activeFrom,
                IIdentityFactory<Guid> identityFactory)
                : base(amount, decimalPlaces, remark, activeFrom, identityFactory) { }
        }
    }
}