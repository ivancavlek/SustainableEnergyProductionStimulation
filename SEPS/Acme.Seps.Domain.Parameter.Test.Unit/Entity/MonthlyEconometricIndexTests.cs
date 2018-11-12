using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
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

        public void PeriodCannotStartBeforeInitialPeriod()
        {
            var initialPeriodMinusMonth = new DateTime(2007, 07, 01).AddMonths(-1);
            var period = new Period(new MonthlyPeriodFactory(initialPeriodMinusMonth.AddYears(-1), initialPeriodMinusMonth));

            Action action = () => new DummyMonthlyEconometricIndex(
                _amount, _decimalPlaces, _remark, period, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.MonthlyParameterException);
        }

        public void PeriodMustStartBeforeCurrentMonth()
        {
            var currentDate = DateTime.Now;
            var period = new Period(new MonthlyPeriodFactory(currentDate.AddMonths(-1), DateTime.Now));

            Action action = () => new DummyMonthlyEconometricIndex(
                _amount, _decimalPlaces, _remark, period, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.MonthlyParameterException);
        }

        public void PeriodIsCorrectlySet()
        {
            var correctDate = DateTime.UtcNow.AddMonths(-1);
            var period = new Period(new MonthlyPeriodFactory(correctDate.AddMonths(-2), correctDate));

            Action action = () => new DummyMonthlyEconometricIndex(
                _amount, _decimalPlaces, _remark, period, _identityFactory);

            action
                .Should()
                .NotThrow<Exception>();
        }
    }

    internal class DummyMonthlyEconometricIndex : MonthlyEconometricIndex<DummyMonthlyEconometricIndex>
    {
        public DummyMonthlyEconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            Period period,
            IIdentityFactory<Guid> identityFactory)
            : base(amount, decimalPlaces, remark, period, identityFactory) { }

        public override DummyMonthlyEconometricIndex CreateNew(
            decimal amount, string remark, int month, int year, IIdentityFactory<Guid> identityFactory) =>
            this;
    }
}