using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Test.Unit.Utility.Factory;
using Acme.Seps.Text;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity;

public class RenewableEnergySourceTariffTests
{
    private readonly IIdentityFactory<Guid> _identityFactory;
    private IEconometricIndexFactory<ConsumerPriceIndex> _cpiFactory;
    private IResTariffFactory<RenewableEnergySourceTariff> _resFactory;

    public RenewableEnergySourceTariffTests()
    {
        _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        _cpiFactory = new EconometricIndexFactory<ConsumerPriceIndex>(DateTime.Now.AddYears(-3));
        _resFactory = new ResTariffFactory(_cpiFactory.Create());
    }

    public void ConsumerPriceIndexMustBeSet()
    {
        Action action = () => _resFactory.Create().CreateNewWith(null, _identityFactory);

        action
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithMessage(SepsMessage.EntityNotSet("consumerPriceIndex"));
    }

    public void ConsumerPriceIndexMustBeActive()
    {
        var cpi = _cpiFactory.Create();
        typeof(ConsumerPriceIndex)
            .BaseType.BaseType.BaseType
            .GetMethod("SetInactive", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(cpi, new object[] { DateTimeOffset.Now });

        Action action = () => _resFactory.Create().CreateNewWith(cpi, _identityFactory);

        action
            .Should()
            .Throw<Exception>()
            .WithMessage(SepsMessage.InactiveException("consumerPriceIndex"));
    }

    public void CreatesProperly()
    {
        var activeRenewableEnergySourceTariff = _resFactory.Create();

        _cpiFactory = new EconometricIndexFactory<ConsumerPriceIndex>(DateTime.Now.AddYears(-2));
        var newConsumerPriceIndex = _cpiFactory.Create();

        var newRenewableEnergySourceTariff =
            activeRenewableEnergySourceTariff.CreateNewWith(newConsumerPriceIndex, _identityFactory);

        activeRenewableEnergySourceTariff.Active.Until.Should().Be(newConsumerPriceIndex.Active.Since);
        newRenewableEnergySourceTariff.LowerRate.Should().Be(activeRenewableEnergySourceTariff.LowerRate);
        newRenewableEnergySourceTariff.HigherRate.Should().Be(
            (newConsumerPriceIndex.Amount / 100M) * activeRenewableEnergySourceTariff.HigherRate);
        newRenewableEnergySourceTariff.ConsumerPriceIndex.Should().Be(newConsumerPriceIndex);
        newRenewableEnergySourceTariff.Active.Should().Be(newConsumerPriceIndex.Active);
    }

    public void ConsumerPriceIndexIsCorrected()
    {
        var previousRes = _resFactory.Create();

        _cpiFactory = new EconometricIndexFactory<ConsumerPriceIndex>(DateTime.Now.AddYears(-2));
        _resFactory = new ResTariffFactory(_cpiFactory.Create());
        var activeRes = _resFactory.Create();
        var correctedConsumerPriceIndex = _cpiFactory.Create();

        activeRes.CpiCorrection(correctedConsumerPriceIndex, previousRes);

        activeRes.Active.Since.Should().Be(correctedConsumerPriceIndex.Active.Since);
        activeRes.LowerRate.Should().Be(previousRes.LowerRate);
        activeRes.HigherRate.Should().Be(
            (correctedConsumerPriceIndex.Amount / 100M) * previousRes.HigherRate);
    }
}