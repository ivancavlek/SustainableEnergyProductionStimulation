﻿using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Text;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity;

public class TariffTests
{
    private readonly int _lowerProductionLimit;
    private readonly int _higherProductionLimit;
    private readonly decimal _lowerRate;
    private readonly decimal _higherRate;
    private readonly Guid _projectTypeId;
    private readonly DateTimeOffset _activeFrom;
    private readonly IIdentityFactory<Guid> _identityFactory;

    public TariffTests()
    {
        _lowerProductionLimit = 1;
        _higherProductionLimit = 2;
        _lowerRate = 1M;
        _higherRate = 2M;
        _projectTypeId = Guid.NewGuid();
        _activeFrom = DateTime.UtcNow;
        _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
    }

    public void LowerProductionLimitMustBeAPositiveNumber()
    {
        Action action = () => new DummyTariff(
            -1,
            _higherProductionLimit,
            _lowerRate,
            _higherRate,
            _projectTypeId,
            _activeFrom,
            _identityFactory);

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.ValueZeroOrAbove("lowerProductionLimit"));
    }

    public void UpperProductionLimitMustBeAPositiveNumber()
    {
        Action action = () => new DummyTariff(
            _lowerProductionLimit,
            -1,
            _lowerRate,
            _higherRate,
            _projectTypeId,
            _activeFrom,
            _identityFactory);

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.ValueZeroOrAbove("upperProductionLimit"));
    }

    public void LowerProductionLimitMustBeLowerOrEqualHigherProductionLimit()
    {
        Action action = () => new DummyTariff(
            2,
            1,
            _lowerRate,
            _higherRate,
            _projectTypeId,
            _activeFrom,
            _identityFactory);

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.ValueHigherThanTheOther("upperProductionLimit", "lowerProductionLimit"));
    }

    public void LowerRateMustBeAPositiveNumber()
    {
        Action action = () => new DummyTariff(
            _lowerProductionLimit,
            _higherProductionLimit,
            -1M,
            _higherRate,
            _projectTypeId,
            _activeFrom,
            _identityFactory);

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.ValueZeroOrAbove("lowerRate"));
    }

    public void HigherRateMustBeAPositiveNumber()
    {
        Action action = () => new DummyTariff(
            _lowerProductionLimit,
            _higherProductionLimit,
            _lowerRate,
            -1M,
            _projectTypeId,
            _activeFrom,
            _identityFactory);

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.ValueZeroOrAbove("higherRate"));
    }

    public void LowerRateMustBeLowerOrEqualHigherRate()
    {
        Action action = () => new DummyTariff(
            _lowerProductionLimit,
            _higherProductionLimit,
            2M,
            1M,
            _projectTypeId,
            _activeFrom,
            _identityFactory);

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.ValueHigherThanTheOther("higherRate", "lowerRate"));
    }

    public void ProjectTypeIdentifierMustBeANonDefaultId()
    {
        Action action = () => new DummyTariff(
            _lowerProductionLimit,
            _higherProductionLimit,
            _lowerRate,
            _higherRate,
            Guid.Empty,
            _activeFrom,
            _identityFactory);

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.EntityNotSet("projectTypeId"));
    }

    public void ProjectTypeIdentifierMustBeNonEmpty()
    {
        Action action = () => new DummyTariff(
            _lowerProductionLimit,
            _higherProductionLimit,
            _lowerRate,
            _higherRate,
            new Guid(),
            _activeFrom,
            _identityFactory);

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.EntityNotSet("projectTypeId"));
    }

    public void TariffIsProperlySet()
    {
        var result = new DummyTariff(
            _lowerProductionLimit,
            _higherProductionLimit,
            _lowerRate,
            _higherRate,
            _projectTypeId,
            _activeFrom,
            _identityFactory); ;

        result.LowerRate.Should().Be(_lowerRate);
        result.HigherRate.Should().Be(_higherRate);
    }

    private class DummyTariff : Tariff
    {
        public DummyTariff(
            int? lowerProductionLimit,
            int? higherProductionLimit,
            decimal lowerRate,
            decimal higherRate,
            Guid projectTypeId,
            DateTimeOffset since,
            IIdentityFactory<Guid> identityFactory)
            : base(lowerProductionLimit,
                  higherProductionLimit,
                  lowerRate,
                  higherRate,
                  projectTypeId,
                  since,
                  identityFactory)
        {
        }
    }
}