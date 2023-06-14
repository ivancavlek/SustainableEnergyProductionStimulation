using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using NSubstitute;
using System;
using System.Reflection;

namespace Acme.Seps.Test.Unit.Utility.Factory;

public sealed class ResTariffFactory : IResTariffFactory<RenewableEnergySourceTariff>
{
    private readonly ConsumerPriceIndex _consumerPriceIndex;

    public ResTariffFactory(ConsumerPriceIndex consumerPriceIndex) => _consumerPriceIndex = consumerPriceIndex ?? throw new ArgumentNullException(nameof(consumerPriceIndex));

    RenewableEnergySourceTariff IResTariffFactory<RenewableEnergySourceTariff>.Create() =>
        Activator.CreateInstance(
            typeof(RenewableEnergySourceTariff),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new object[]
            {
                _consumerPriceIndex,
                100M,
                500M,
                10M,
                10M,
                Guid.NewGuid(),
                Substitute.For<IIdentityFactory<Guid>>() },
            null) as RenewableEnergySourceTariff;
}

public interface IResTariffFactory<TTariff> where TTariff : Tariff
{
    TTariff Create();
}