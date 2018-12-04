using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using NSubstitute;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Factory
{
    internal class TariffFactory<TTariff> : ITariffFactory<TTariff> where TTariff : Tariff
    {
        private readonly IPeriodFactory _periodFactory;
        private readonly EconometricIndex _econometricIndex;

        public TariffFactory(EconometricIndex econometricIndex, IPeriodFactory periodFactory)
        {
            _periodFactory = periodFactory ?? throw new ArgumentNullException(nameof(periodFactory));
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
                    _periodFactory,
                    Substitute.For<IIdentityFactory<Guid>>() },
                null) as TTariff;
    }
}