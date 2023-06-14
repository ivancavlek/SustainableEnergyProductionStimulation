using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using NSubstitute;
using System;
using System.Reflection;

namespace Acme.Seps.Test.Unit.Utility.Factory;

public sealed class EconometricIndexFactory<TEconomtricIndex>
    : IEconometricIndexFactory<TEconomtricIndex> where TEconomtricIndex : EconometricIndex
{
    private readonly DateTimeOffset _activeFrom;
    private readonly IIdentityFactory<Guid> _identityFactory;

    public EconometricIndexFactory(DateTimeOffset since)
    {
        _activeFrom = since;
        _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        _identityFactory.CreateIdentity().Returns(Guid.NewGuid());
    }

    TEconomtricIndex IEconometricIndexFactory<TEconomtricIndex>.Create() =>
        Activator.CreateInstance(
            typeof(TEconomtricIndex),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new object[] { 100M, nameof(TEconomtricIndex), _activeFrom, _identityFactory },
            null) as TEconomtricIndex;
}

public interface IEconometricIndexFactory<TEconometricIndex> where TEconometricIndex : EconometricIndex
{
    TEconometricIndex Create();
}