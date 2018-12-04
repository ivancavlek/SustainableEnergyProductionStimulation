using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Subsidy.Entity;
using NSubstitute;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Factory
{
    internal class EconometricIndexFactory<TEconomtricIndex>
        : IEconometricIndexFactory<TEconomtricIndex> where TEconomtricIndex : EconometricIndex
    {
        private readonly IPeriodFactory _periodFactory;

        public EconometricIndexFactory(IPeriodFactory periodFactory) =>
            _periodFactory = periodFactory ?? throw new ArgumentNullException(nameof(periodFactory));

        TEconomtricIndex IEconometricIndexFactory<TEconomtricIndex>.Create() =>
            Activator.CreateInstance(
                typeof(TEconomtricIndex),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[]
                {
                    10M, nameof(TEconomtricIndex), new Period(_periodFactory), Substitute.For<IIdentityFactory<Guid>>()
                },
                null) as TEconomtricIndex;
    }
}