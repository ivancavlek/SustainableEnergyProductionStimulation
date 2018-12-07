using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class EconometricIndexTests
    {
        private readonly decimal _amount;
        private readonly int _decimalPlaces;
        private readonly string _remark;
        private readonly DateTimeOffset _activeFrom;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public EconometricIndexTests()
        {
            _amount = 1M;
            _decimalPlaces = 2;
            _remark = nameof(_remark);
            _activeFrom = DateTime.UtcNow;
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        }

        public void AmountCannotBeANegativeValue()
        {
            Action action = () =>
                new DummyEconometricIndex(-1M, _decimalPlaces, _remark, _activeFrom, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.ParameterAmountBelowOrZeroException);
        }

        public void AmountCannotBeZeroBased()
        {
            Action action = () =>
                new DummyEconometricIndex(0M, _decimalPlaces, _remark, _activeFrom, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.ParameterAmountBelowOrZeroException);
        }

        public void AmountIsRoundedAtHigherValueWithDefinedPrecisionDigits()
        {
            const decimal amount = 1.2578M;

            var result = new DummyEconometricIndex(amount, _decimalPlaces, _remark, _activeFrom, _identityFactory);

            result.Amount.Should().Be(Math.Round(amount, _decimalPlaces, MidpointRounding.AwayFromZero));
        }

        public void DecimalPlacesCannotBeANegativeNumber()
        {
            Action action = () =>
                new DummyEconometricIndex(_amount, -1, _remark, _activeFrom, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.ParameterDecimalPlacesBelowZeroException);
        }

        public void RemarkMustExist()
        {
            Action action = () =>
                new DummyEconometricIndex(_amount, _decimalPlaces, string.Empty, _activeFrom, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.RemarkNotSetException);
        }

        public void CreatesProperly()
        {
            var econometricIndex =
                new DummyEconometricIndex(_amount, _decimalPlaces, _remark, _activeFrom, _identityFactory);

            econometricIndex.Remark.Should().Be(_remark);
        }

        private class DummyEconometricIndex : EconometricIndex
        {
            public DummyEconometricIndex(
                decimal amount,
                int decimalPlaces,
                string remark,
                DateTimeOffset activeFrom,
                IIdentityFactory<Guid> identityFactory)
                : base(amount, decimalPlaces, remark, activeFrom, identityFactory)
            {
            }
        }
    }
}