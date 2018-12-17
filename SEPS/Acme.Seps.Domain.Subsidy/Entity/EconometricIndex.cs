using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public abstract class EconometricIndex : SepsAggregateRoot
    {
        protected abstract int DecimalPlaces { get; }

        public decimal Amount { get; private set; }
        public string Remark { get; private set; }

        protected EconometricIndex() { }

        protected EconometricIndex(
            decimal amount, string remark, DateTimeOffset activeFrom, IIdentityFactory<Guid> identityFactory)
            : base(activeFrom, identityFactory)
        {
            amount.MustBeGreaterThan(0m, (_, __) =>
                new DomainException(SubsidyMessages.ParameterAmountBelowOrZeroException));
            remark.MustNotBeNullOrWhiteSpace((_) => new DomainException(SubsidyMessages.RemarkNotSetException));

            Amount = RoundAmount(amount);
            Remark = remark;
        }

        protected void AmountCorrection(decimal amount, string remark)
        {
            if (Active.Since.Equals(SepsVersion.InitialDate()))
                throw new DomainException(SubsidyMessages.InitialValuesMustNotBeChanged);
            amount.MustBeGreaterThan(0m, (_, __) =>
                new DomainException(SubsidyMessages.ParameterAmountBelowOrZeroException));
            remark.MustNotBeNullOrWhiteSpace((_) => new DomainException(SubsidyMessages.RemarkNotSetException));

            Amount = RoundAmount(amount);
            Remark = remark;
        }

        private decimal RoundAmount(decimal amount) =>
            Math.Round(amount, DecimalPlaces, MidpointRounding.AwayFromZero);
    }
}