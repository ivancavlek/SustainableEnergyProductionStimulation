using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Text;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity;

public abstract class EconometricIndex : SepsAggregateRoot
{
    protected abstract int DecimalPlaces { get; }

    public decimal Amount { get; private set; }
    public string Remark { get; private set; }

    protected EconometricIndex() { }

    protected EconometricIndex(
        decimal amount, string remark, DateTimeOffset since, IIdentityFactory<Guid> identityFactory)
        : base(since, identityFactory)
    {
        amount.MustBeGreaterThan(0m, (_, __) =>
            new DomainException(SepsMessage.ValueZeroOrAbove(nameof(amount))));
        remark.MustNotBeNullOrWhiteSpace((_) => new DomainException(SepsMessage.EntityNotSet(nameof(remark))));

        Amount = RoundAmount(amount);
        Remark = remark;
    }

    protected void AmountCorrection(decimal amount, string remark)
    {
        if (Active.Since.Equals(SepsVersion.InitialDate()))
            throw new DomainException(SepsMessage.InitialValuesMustNotBeChanged());
        amount.MustBeGreaterThan(0m, (_, __) =>
            new DomainException(SepsMessage.ValueZeroOrAbove(nameof(amount))));
        remark.MustNotBeNullOrWhiteSpace((_) => new DomainException(SepsMessage.EntityNotSet(nameof(remark))));

        Amount = RoundAmount(amount);
        Remark = remark;
    }

    private decimal RoundAmount(decimal amount) =>
        Math.Round(amount, DecimalPlaces, MidpointRounding.AwayFromZero);
}