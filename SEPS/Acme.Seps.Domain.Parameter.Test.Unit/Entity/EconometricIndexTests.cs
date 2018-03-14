using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class EconometricIndexTests
    {
        private readonly decimal _amount;
        private readonly int _decimalPlaces;
        private readonly string _remark;
        private readonly Period _period;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public EconometricIndexTests()
        {
            _amount = 1M;
            _decimalPlaces = 2;
            _remark = nameof(_remark);
            _period = new MonthlyPeriod(DateTime.UtcNow);
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        }

        public void AmountCannotBeANegativeValue()
        {
            Action action = () =>
                new DummyEconometricIndex(-1M, _decimalPlaces, _remark, _period, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.ParameterAmountBelowOrZeroException);
        }

        public void AmountCannotBeZeroBased()
        {
            Action action = () =>
                new DummyEconometricIndex(0M, _decimalPlaces, _remark, _period, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.ParameterAmountBelowOrZeroException);
        }

        public void AmountIsRoundedAtHigherValueWithDefinedPrecisionDigits()
        {
            var amount = 1.2578M;

            var result = new DummyEconometricIndex(amount, _decimalPlaces, _remark, _period, _identityFactory);

            result.Amount.Should().Be(Math.Round(amount, _decimalPlaces, MidpointRounding.AwayFromZero));
        }

        public void DecimalPlacesCannotBeANegativeNumber()
        {
            Action action = () =>
                new DummyEconometricIndex(_amount, -1, _remark, _period, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.ParameterDecimalPlacesBelowZeroException);
        }

        public void RemarkMustExist()
        {
            Action action = () =>
                new DummyEconometricIndex(_amount, _decimalPlaces, string.Empty, _period, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.RemarkNotSetException);
        }
    }

    internal class DummyEconometricIndex : EconometricIndex
    {
        public DummyEconometricIndex(
            decimal amount, int decimalPlaces, string remark, Period period, IIdentityFactory<Guid> identityFactory)
            : base(amount, decimalPlaces, remark, period, identityFactory) { }
    }
}