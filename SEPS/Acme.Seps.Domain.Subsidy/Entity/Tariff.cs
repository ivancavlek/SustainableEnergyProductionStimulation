using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public abstract class Tariff : SepsBaseAggregate
    {
        public int LowerProductionLimit { get; private set; }
        public int UpperProductionLimit { get; private set; }
        public decimal LowerRate { get; protected set; }
        public decimal HigherRate { get; protected set; }

        protected Tariff() { }

        protected Tariff(
            int lowerProductionLimit,
            int upperProductionLimit,
            decimal lowerRate,
            decimal higherRate,
            IPeriodFactory periodFactory,
            IIdentityFactory<Guid> identityFactory) : base(periodFactory, identityFactory)
        {
            lowerProductionLimit.MustBeGreaterThanOrEqualTo(0, (_, __) =>
                new DomainException(SubsidyMessages.BelowZeroLowerProductionLimitException));
            upperProductionLimit.MustBeGreaterThanOrEqualTo(0, (_, __) =>
                new DomainException(SubsidyMessages.BelowZeroUpperProductionLimitException));
            lowerProductionLimit.MustBeLessThanOrEqualTo(upperProductionLimit, (_, __) =>
                new DomainException(SubsidyMessages.LowerProductionLimitAboveUpperProductionLimitException));
            lowerRate.MustBeGreaterThanOrEqualTo(0m, (_, __) =>
                new DomainException(SubsidyMessages.BelowZeroLowerRateException));
            higherRate.MustBeGreaterThanOrEqualTo(0m, (_, __) =>
                new DomainException(SubsidyMessages.BelowZeroUpperRateException));
            lowerRate.MustBeLessThanOrEqualTo(higherRate, (_, __) =>
                new DomainException(SubsidyMessages.LowerRateAboveUpperException));

            LowerProductionLimit = lowerProductionLimit;
            UpperProductionLimit = upperProductionLimit;
            LowerRate = lowerRate;
            HigherRate = higherRate;
        }
    }
}