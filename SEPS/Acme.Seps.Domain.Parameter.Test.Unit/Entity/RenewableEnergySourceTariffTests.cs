﻿using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using Moq;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class RenewableEnergySourceTariffTests
    {
        private readonly RenewableEnergySourceTariff _existingRes;
        private readonly ConsumerPriceIndex _consumerPriceIndex;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;

        private readonly decimal _higherRate;
        private readonly YearlyPeriod _resPeriod;

        public RenewableEnergySourceTariffTests()
        {
            _higherRate = 10M;
            _resPeriod = new YearlyPeriod(DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-3));
            _identityFactory = new Mock<IIdentityFactory<Guid>>();

            var resConsumerPriceIndex = new ConsumerPriceIndex(
                100M,
                nameof(ConsumerPriceIndex),
                _resPeriod,
                _identityFactory.Object);
            _consumerPriceIndex = new ConsumerPriceIndex(
                105M,
                nameof(ConsumerPriceIndex),
                new YearlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                _identityFactory.Object);

            _existingRes = Activator.CreateInstance(
                typeof(RenewableEnergySourceTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { resConsumerPriceIndex, 5M, _higherRate, _identityFactory.Object },
                null) as RenewableEnergySourceTariff;
        }

        public void ConsumerPriceIndexMustBeSet()
        {
            Action action = () => _existingRes.CreateNewWith(null, _identityFactory.Object);

            action
                .ShouldThrowExactly<ArgumentNullException>()
                .WithMessage(Infrastructure.Parameter.ConsumerPriceIndexNotSetException);
        }

        public void CpiPeriodMustFollowResPeriod()
        {
            var resConsumerPriceIndex = new ConsumerPriceIndex(
                100M,
                nameof(ConsumerPriceIndex),
                new YearlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                _identityFactory.Object);
            var falseRes = Activator.CreateInstance(
                typeof(RenewableEnergySourceTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { resConsumerPriceIndex, 5M, _higherRate, _identityFactory.Object },
                null) as RenewableEnergySourceTariff;

            Action action = () => falseRes.CreateNewWith(_consumerPriceIndex, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.RenewableEnergySourceTariffPeriodException);
        }

        public void ResIsCorrectlyConstructed()
        {
            var result = _existingRes.CreateNewWith(_consumerPriceIndex, _identityFactory.Object);

            _existingRes.Period.ValidTill.Should().Be(_consumerPriceIndex.Period.ValidFrom);
            result.ConsumerPriceIndex.Should().Be(_consumerPriceIndex);
            result.Period.Should().Be(_consumerPriceIndex.Period);
            result.LowerRate.Should().Be(_existingRes.LowerRate);
            result.HigherRate.Should().Be((_consumerPriceIndex.Amount / 100M) * _existingRes.HigherRate);
        }
    }
}