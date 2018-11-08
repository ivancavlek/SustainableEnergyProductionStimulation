using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class TariffTests
    {
        private readonly decimal _lowerRate;
        private readonly decimal _higherRate;
        private readonly MonthlyPeriod _monthlyPeriod;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public TariffTests()
        {
            _lowerRate = 1M;
            _higherRate = 2M;
            _monthlyPeriod = new MonthlyPeriod(DateTime.UtcNow);
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        }

        public void LowerRateMustBeAPositiveNumber()
        {
            Action action = () =>
                new DummyTariff(-1M, _higherRate, _monthlyPeriod, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.BelowZeroLowerRateException);
        }

        public void HigherRateMustBeAPositiveNumber()
        {
            Action action = () =>
                new DummyTariff(_lowerRate, -1M, _monthlyPeriod, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.BelowZeroUpperRateException);
        }

        public void LowerRateMustBeLowerOrEqualHigherRate()
        {
            Action action = () =>
                new DummyTariff(2M, 1M, _monthlyPeriod, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.LowerRateAboveUpperException);
        }

        public void TariffIsProperlySet()
        {
            var result = new DummyTariff(_lowerRate, _higherRate, _monthlyPeriod, _identityFactory);

            result.LowerRate.Should().Be(_lowerRate);
            result.HigherRate.Should().Be(_higherRate);
            result.MonthlyPeriod.Should().Be(_monthlyPeriod);
        }
    }

    internal class DummyTariff : Tariff
    {
        public MonthlyPeriod MonthlyPeriod { get; private set; }

        public DummyTariff(
            decimal lowerRate, decimal higherRate, MonthlyPeriod period, IIdentityFactory<Guid> identityFactory)
            : base(lowerRate, higherRate, identityFactory)
        {
            MonthlyPeriod = period;
        }
    }
}