using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Text;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity;

public abstract class Tariff : SepsAggregateRoot
{
    public decimal? LowerProductionLimit { get; private set; }
    public decimal? UpperProductionLimit { get; private set; }
    public decimal LowerRate { get; protected set; }
    public decimal HigherRate { get; protected set; }
    public Guid ProjectTypeId { get; private set; }

    protected Tariff() { }

    protected Tariff(
        decimal? lowerProductionLimit,
        decimal? upperProductionLimit,
        decimal lowerRate,
        decimal higherRate,
        Guid projectTypeId,
        DateTimeOffset since,
        IIdentityFactory<Guid> identityFactory) : base(since, identityFactory)
    {
        if (lowerProductionLimit.HasValue)
            lowerProductionLimit.Value.MustBeGreaterThanOrEqualTo(0, (_, __) =>
                new DomainException(SepsMessage.ValueZeroOrAbove(nameof(lowerProductionLimit))));
        if (upperProductionLimit.HasValue)
        {
            upperProductionLimit.Value.MustBeGreaterThanOrEqualTo(0, (_, __) =>
               new DomainException(SepsMessage.ValueZeroOrAbove(nameof(upperProductionLimit))));
            lowerProductionLimit.Value.MustBeLessThanOrEqualTo(upperProductionLimit.Value, (_, __) =>
                new DomainException(SepsMessage.ValueHigherThanTheOther(nameof(upperProductionLimit), nameof(lowerProductionLimit))));
        }
        lowerRate.MustBeGreaterThanOrEqualTo(0m, (_, __) =>
            new DomainException(SepsMessage.ValueZeroOrAbove(nameof(lowerRate))));
        higherRate.MustBeGreaterThanOrEqualTo(0m, (_, __) =>
            new DomainException(SepsMessage.ValueZeroOrAbove(nameof(higherRate))));
        lowerRate.MustBeLessThanOrEqualTo(higherRate, (_, __) =>
            new DomainException(SepsMessage.ValueHigherThanTheOther(nameof(higherRate), nameof(lowerRate))));
        projectTypeId.MustNotBeEmpty(() =>
            new DomainException(SepsMessage.EntityNotSet(nameof(projectTypeId))));
        projectTypeId.MustNotBeDefault(() =>
            new DomainException(SepsMessage.EntityNotSet(nameof(projectTypeId))));

        LowerProductionLimit = lowerProductionLimit;
        UpperProductionLimit = upperProductionLimit;
        LowerRate = lowerRate;
        HigherRate = higherRate;
        ProjectTypeId = projectTypeId;
    }
}