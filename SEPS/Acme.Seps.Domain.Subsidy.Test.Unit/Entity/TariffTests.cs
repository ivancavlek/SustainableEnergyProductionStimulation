using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class TariffTests
    {
        private readonly int _lowerProductionLimit;
        private readonly int _higherProductionLimit;
        private readonly decimal _lowerRate;
        private readonly decimal _higherRate;
        private readonly Guid _projectTypeId;
        private readonly IPeriodFactory _monthlyPeriod;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public TariffTests()
        {
            _lowerProductionLimit = 1;
            _higherProductionLimit = 2;
            _lowerRate = 1M;
            _higherRate = 2M;
            _projectTypeId = Guid.NewGuid();
            _monthlyPeriod = new MonthlyPeriodFactory(DateTime.UtcNow);
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        }

        public void LowerProductionLimitMustHaveAValue()
        {
            Action action = () => new DummyTariff(
                null,
                _higherProductionLimit,
                _lowerRate,
                _higherRate,
                _projectTypeId,
                _monthlyPeriod,
                _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.BelowZeroLowerProductionLimitException);
        }

        public void LowerProductionLimitMustBeAPositiveNumber()
        {
            Action action = () => new DummyTariff(
                -1,
                _higherProductionLimit,
                _lowerRate,
                _higherRate,
                _projectTypeId,
                _monthlyPeriod,
                _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.BelowZeroLowerProductionLimitException);
        }

        public void UpperProductionLimitMustHaveAValue()
        {
            Action action = () => new DummyTariff(
                _lowerProductionLimit,
                null,
                _lowerRate,
                _higherRate,
                _projectTypeId,
                _monthlyPeriod,
                _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.BelowZeroUpperProductionLimitException);
        }

        public void UpperProductionLimitMustBeAPositiveNumber()
        {
            Action action = () => new DummyTariff(
                _lowerProductionLimit,
                -1,
                _lowerRate,
                _higherRate,
                _projectTypeId,
                _monthlyPeriod,
                _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.BelowZeroUpperProductionLimitException);
        }

        public void LowerProductionLimitMustBeLowerOrEqualHigherProductionLimit()
        {
            Action action = () => new DummyTariff(
                2,
                1,
                _lowerRate,
                _higherRate,
                _projectTypeId,
                _monthlyPeriod,
                _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.LowerProductionLimitAboveUpperProductionLimitException);
        }

        public void LowerRateMustBeAPositiveNumber()
        {
            Action action = () => new DummyTariff(
                _lowerProductionLimit,
                _higherProductionLimit,
                -1M,
                _higherRate,
                _projectTypeId,
                _monthlyPeriod,
                _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.BelowZeroLowerRateException);
        }

        public void HigherRateMustBeAPositiveNumber()
        {
            Action action = () => new DummyTariff(
                _lowerProductionLimit,
                _higherProductionLimit,
                _lowerRate,
                -1M,
                _projectTypeId,
                _monthlyPeriod,
                _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.BelowZeroUpperRateException);
        }

        public void LowerRateMustBeLowerOrEqualHigherRate()
        {
            Action action = () => new DummyTariff(
                _lowerProductionLimit,
                _higherProductionLimit,
                2M,
                1M,
                _projectTypeId,
                _monthlyPeriod,
                _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.LowerRateAboveUpperException);
        }

        public void ProjectTypeIdentifierMustBeANonDefaultId()
        {
            Action action = () => new DummyTariff(
                _lowerProductionLimit,
                _higherProductionLimit,
                _lowerRate,
                -1M,
                Guid.Empty,
                _monthlyPeriod,
                _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.BelowZeroUpperRateException);
        }

        public void ProjectTypeIdentifierMustBeNonEmpty()
        {
            Action action = () => new DummyTariff(
                _lowerProductionLimit,
                _higherProductionLimit,
                2M,
                1M,
                new Guid(),
                _monthlyPeriod,
                _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.LowerRateAboveUpperException);
        }

        public void TariffIsProperlySet()
        {
            var result = new DummyTariff(
                _lowerProductionLimit,
                _higherProductionLimit,
                _lowerRate,
                _higherRate,
                _projectTypeId,
                _monthlyPeriod,
                _identityFactory); ;

            result.LowerRate.Should().Be(_lowerRate);
            result.HigherRate.Should().Be(_higherRate);
        }
    }

    internal class DummyTariff : Tariff
    {
        public DummyTariff(
            int? lowerProductionLimit,
            int? higherProductionLimit,
            decimal lowerRate,
            decimal higherRate,
            Guid projectTypeId,
            IPeriodFactory period,
            IIdentityFactory<Guid> identityFactory)
            : base(lowerProductionLimit,
                  higherProductionLimit,
                  lowerRate,
                  higherRate,
                  projectTypeId,
                  period,
                  identityFactory)
        {
        }
    }
}