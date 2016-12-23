using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using Moq;
using System;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class YearlyEconometricIndexTests
    {
        private readonly decimal _amount;
        private readonly int _decimalPlaces;
        private readonly string _remark;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;

        public YearlyEconometricIndexTests()
        {
            _amount = 1M;
            _decimalPlaces = 2;
            _remark = nameof(_remark);
            _identityFactory = new Mock<IIdentityFactory<Guid>>();
        }

        public void PeriodCannotStartBeforeInitialPeriod()
        {
            var initialPeriodMinusYear = new DateTime(2007, 07, 01).AddYears(-1);
            var period = new YearlyPeriod(initialPeriodMinusYear.AddYears(-2), initialPeriodMinusYear);

            Action action = () => new DummyYearlyEconometricIndex(
                _amount, _decimalPlaces, _remark, period, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.YearlyParameterException);
        }

        public void PeriodMustStartBeforeCurrentYear()
        {
            var currentDate = DateTime.UtcNow;
            var period = new YearlyPeriod(currentDate.AddYears(-1), DateTime.UtcNow);

            Action action = () => new DummyYearlyEconometricIndex(
                _amount, _decimalPlaces, _remark, period, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.YearlyParameterException);
        }

        public void PeriodIsCorrectlySet()
        {
            var correctDate = DateTime.UtcNow.AddYears(-2);
            var period = new YearlyPeriod(correctDate.AddYears(-1), correctDate);

            Action action = () => new DummyYearlyEconometricIndex(
                _amount, _decimalPlaces, _remark, period, _identityFactory.Object);

            action
                .ShouldNotThrow<Exception>();
        }
    }

    internal class DummyYearlyEconometricIndex : YearlyEconometricIndex
    {
        public DummyYearlyEconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            YearlyPeriod period,
            IIdentityFactory<Guid> identityFactory)
            : base(amount, decimalPlaces, remark, period, identityFactory) { }

        public override YearlyEconometricIndex CreateNew(
            decimal amount, string remark, IIdentityFactory<Guid> identityFactory) =>
            this;
    }
}