using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using Acme.Seps.Domain.Subsidy.Test.Unit.Factory;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class RenewableEnergySourceTariffTests
    {
        private readonly IIdentityFactory<Guid> _identityFactory;
        private IEconometricIndexFactory<ConsumerPriceIndex> _cpiFactory;
        private ITariffFactory<RenewableEnergySourceTariff> _resFactory;

        public RenewableEnergySourceTariffTests()
        {
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        }

        public void ConsumerPriceIndexMustBeSet()
        {
            _cpiFactory = new EconometricIndexFactory<ConsumerPriceIndex>(DateTime.Now.AddYears(-3));
            _resFactory = new TariffFactory<RenewableEnergySourceTariff>(
                _cpiFactory.Create(), DateTime.Now.AddYears(-4));

            Action action = () => _resFactory.Create().CreateNewWith(null, _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentNullException>()
                .WithMessage(SubsidyMessages.ConsumerPriceIndexNotSetException);
        }

        public void CreatesProperly()
        {
            DateTimeOffset activeFrom = DateTimeOffset.Now.ToFirstMonthOfTheYear().AddYears(-3);
            _cpiFactory = new EconometricIndexFactory<ConsumerPriceIndex>(activeFrom);
            _resFactory = new TariffFactory<RenewableEnergySourceTariff>(_cpiFactory.Create(), activeFrom);
            var activeRenewableEnergySourceTariff = _resFactory.Create();

            _cpiFactory = new EconometricIndexFactory<ConsumerPriceIndex>(DateTime.Now.AddYears(-2));
            var newConsumerPriceIndex = _cpiFactory.Create();

            var newRenewableEnergySourceTariff =
                activeRenewableEnergySourceTariff.CreateNewWith(newConsumerPriceIndex, _identityFactory);

            activeRenewableEnergySourceTariff.Period.ActiveTill.Should().Be(newConsumerPriceIndex.Period.ActiveFrom);
            newRenewableEnergySourceTariff.LowerRate.Should().Be(activeRenewableEnergySourceTariff.LowerRate);
            newRenewableEnergySourceTariff.HigherRate.Should().Be(
                (newConsumerPriceIndex.Amount / 100M) * activeRenewableEnergySourceTariff.HigherRate);
            newRenewableEnergySourceTariff.ConsumerPriceIndex.Should().Be(newConsumerPriceIndex);
            newRenewableEnergySourceTariff.Period.Should().Be(newConsumerPriceIndex.Period);
        }
    }
}