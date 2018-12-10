using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using NSubstitute;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Factory
{
    internal class TariffFactory<TTariff> : ITariffFactory<TTariff> where TTariff : Tariff
    {
        private readonly EconometricIndex _econometricIndex;

        public TariffFactory(EconometricIndex econometricIndex)
        {
            _econometricIndex = econometricIndex ?? throw new ArgumentNullException(nameof(econometricIndex));
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
                    Substitute.For<IIdentityFactory<Guid>>() },
                null) as TTariff;
    }
}