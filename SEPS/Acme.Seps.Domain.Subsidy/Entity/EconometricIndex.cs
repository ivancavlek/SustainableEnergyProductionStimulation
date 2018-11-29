using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public abstract class EconometricIndex : SepsAggregate
    {
        protected readonly int DecimalPlaces;

        public decimal Amount { get; private set; }
        public string Remark { get; private set; }

        protected EconometricIndex() { }

        protected EconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            IPeriodFactory periodFactory,
            IIdentityFactory<Guid> identityFactory)
            : base(periodFactory, identityFactory)
        {
            amount.MustBeGreaterThan(0m, (_, __) =>
                new DomainException(SubsidyMessages.ParameterAmountBelowOrZeroException));
            decimalPlaces.MustBeGreaterThanOrEqualTo(0, (_, __) =>
                new DomainException(SubsidyMessages.ParameterDecimalPlacesBelowZeroException));
            remark.MustNotBeNullOrWhiteSpace((_) => new DomainException(SubsidyMessages.RemarkNotSetException));

            DecimalPlaces = decimalPlaces;
            Amount = RoundAmount(amount);
            Remark = remark;
        }

        public void AmountCorrection(decimal amount, string remark)
        {
            Amount = RoundAmount(amount);
            Remark = remark;
        }

        private decimal RoundAmount(decimal amount) =>
            Math.Round(amount, DecimalPlaces, MidpointRounding.AwayFromZero);

    }
}