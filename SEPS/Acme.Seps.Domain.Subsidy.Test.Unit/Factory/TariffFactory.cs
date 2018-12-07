using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using NSubstitute;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Factory
{
    internal class TariffFactory<TTariff> : ITariffFactory<TTariff> where TTariff : Tariff
    {
        private readonly DateTimeOffset _activeFrom;
        private readonly EconometricIndex _econometricIndex;

        public TariffFactory(EconometricIndex econometricIndex, DateTimeOffset activeFrom)
        {
            _econometricIndex = econometricIndex ?? throw new ArgumentNullException(nameof(econometricIndex));
            _activeFrom = activeFrom;
        }

        TTariff ITariffFactory<TTariff>.Create() =>
            Activator.CreateInstance(
                typeof(TTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[]
                {
                    _econometricIndex,
                    100,
                    500,
                    10M,
                    10M,
                    Guid.NewGuid(),
                    _activeFrom,
                    Substitute.For<IIdentityFactory<Guid>>() },
                null) as TTariff;
    }
}