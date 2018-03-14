using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using NSubstitute;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class RenewableEnergySourceTariffTests
    {
        private readonly RenewableEnergySourceTariff _existingRes;
        private readonly ConsumerPriceIndex _consumerPriceIndex;
        private readonly IIdentityFactory<Guid> _identityFactory;

        private readonly decimal _higherRate;
        private readonly YearlyPeriod _resPeriod;

        public RenewableEnergySourceTariffTests()
        {
            _higherRate = 10M;
            _resPeriod = new YearlyPeriod(DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-3));
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();

            var resConsumerPriceIndex = Activator.CreateInstance(
                typeof(ConsumerPriceIndex),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    100M,
                    nameof(ConsumerPriceIndex),
                    _resPeriod,
                    _identityFactory },
                null) as ConsumerPriceIndex;
            _consumerPriceIndex = Activator.CreateInstance(
                typeof(ConsumerPriceIndex),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    105M,
                    nameof(ConsumerPriceIndex),
                    new YearlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                    _identityFactory },
                null) as ConsumerPriceIndex;
            _existingRes = Activator.CreateInstance(
                typeof(RenewableEnergySourceTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { resConsumerPriceIndex, 5M, _higherRate, _identityFactory },
                null) as RenewableEnergySourceTariff;
        }

        public void ConsumerPriceIndexMustBeSet()
        {
            Action action = () => _existingRes.CreateNewWith(null, _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentNullException>()
                .WithMessage(Infrastructure.Parameter.ConsumerPriceIndexNotSetException);
        }

        public void CpiPeriodMustFollowResPeriod()
        {
            var resConsumerPriceIndex = (ConsumerPriceIndex)Activator.CreateInstance(
                typeof(ConsumerPriceIndex),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    100M,
                nameof(ConsumerPriceIndex),
                new YearlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                    _identityFactory },
                null);
            var falseRes = Activator.CreateInstance(
                typeof(RenewableEnergySourceTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { resConsumerPriceIndex, 5M, _higherRate, _identityFactory },
                null) as RenewableEnergySourceTariff;

            Action action = () => falseRes.CreateNewWith(_consumerPriceIndex, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.RenewableEnergySourceTariffPeriodException);
        }

        public void ResIsCorrectlyConstructed()
        {
            var result = _existingRes.CreateNewWith(_consumerPriceIndex, _identityFactory);

            _existingRes.Period.ValidTill.Should().Be(_consumerPriceIndex.Period.ValidFrom);
            result.ConsumerPriceIndex.Should().Be(_consumerPriceIndex);
            result.Period.Should().Be(_consumerPriceIndex.Period);
            result.LowerRate.Should().Be(_existingRes.LowerRate);
            result.HigherRate.Should().Be((_consumerPriceIndex.Amount / 100M) * _existingRes.HigherRate);
        }
    }
}