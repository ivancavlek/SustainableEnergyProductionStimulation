using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using Moq;
using System;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class MonthlyEconometricIndexTests
    {
        private readonly decimal _amount;
        private readonly int _decimalPlaces;
        private readonly string _remark;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;

        public MonthlyEconometricIndexTests()
        {
            _amount = 1M;
            _decimalPlaces = 2;
            _remark = nameof(_remark);
            _identityFactory = new Mock<IIdentityFactory<Guid>>();
        }

        public void PeriodCannotStartBeforeInitialPeriod()
        {
            var initialPeriodMinusMonth = new DateTime(2007, 07, 01).AddMonths(-1);
            var period = new MonthlyPeriod(initialPeriodMinusMonth.AddYears(-1), initialPeriodMinusMonth);

            Action action = () => new DummyMonthlyEconometricIndex(
                _amount, _decimalPlaces, _remark, period, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.MonthlyParameterException);
        }

        public void PeriodMustStartBeforeCurrentMonth()
        {
            var currentDate = DateTime.Now;
            var period = new MonthlyPeriod(currentDate.AddMonths(-1), DateTime.Now);

            Action action = () => new DummyMonthlyEconometricIndex(
                _amount, _decimalPlaces, _remark, period, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.MonthlyParameterException);
        }

        public void PeriodIsCorrectlySet()
        {
            var correctDate = DateTime.UtcNow.AddMonths(-1);
            var period = new MonthlyPeriod(correctDate.AddMonths(-2), correctDate);

            Action action = () => new DummyMonthlyEconometricIndex(
                _amount, _decimalPlaces, _remark, period, _identityFactory.Object);

            action
                .ShouldNotThrow<Exception>();
        }
    }

    internal class DummyMonthlyEconometricIndex : MonthlyEconometricIndex
    {
        public DummyMonthlyEconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            MonthlyPeriod period,
            IIdentityFactory<Guid> identityFactory)
            : base(amount, decimalPlaces, remark, period, identityFactory) { }

        public override MonthlyEconometricIndex CreateNew(
            decimal amount, string remark, int month, int year, IIdentityFactory<Guid> identityFactory) =>
            this;
    }
}