using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using Moq;
using System;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class TariffTests
    {
        private readonly decimal _lowerRate;
        private readonly decimal _higherRate;
        private readonly Period _period;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;

        public TariffTests()
        {
            _lowerRate = 1M;
            _higherRate = 2M;
            _period = new MonthlyPeriod(DateTime.UtcNow);
            _identityFactory = new Mock<IIdentityFactory<Guid>>();
        }

        public void LowerRateMustBeAPositiveNumber()
        {
            Action action = () =>
                new DummyTariff(-1M, _higherRate, _period, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.BelowZeroLowerRateException);
        }

        public void HigherRateMustBeAPositiveNumber()
        {
            Action action = () =>
                new DummyTariff(_lowerRate, -1M, _period, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.BelowZeroUpperRateException);
        }

        public void LowerRateMustBeLowerOrEqualHigherRate()
        {
            Action action = () =>
                new DummyTariff(2M, 1M, _period, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.LowerRateAboveUpperException);
        }

        public void TariffIsProperlySet()
        {
            var result = new DummyTariff(_lowerRate, _higherRate, _period, _identityFactory.Object);

            result.LowerRate.Should().Be(_lowerRate);
            result.HigherRate.Should().Be(_higherRate);
            result.Period.Should().Be(_period);
        }
    }

    internal class DummyTariff : Tariff
    {
        public DummyTariff(
            decimal lowerRate, decimal higherRate, Period period, IIdentityFactory<Guid> identityFactory)
            : base(lowerRate, higherRate, period, identityFactory) { }
    }
}