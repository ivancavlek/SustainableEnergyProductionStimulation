using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Factory;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
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
                new DomainException(Message.BelowZeroLowerProductionLimitException));
            upperProductionLimit.MustBeGreaterThanOrEqualTo(0, (_, __) =>
                new DomainException(Message.BelowZeroUpperProductionLimitException));
            lowerProductionLimit.MustBeLessThanOrEqualTo(upperProductionLimit, (_, __) =>
                new DomainException(Message.LowerProductionLimitAboveUpperProductionLimitException));
            lowerRate.MustBeGreaterThanOrEqualTo(0m, (_, __) =>
                new DomainException(Message.BelowZeroLowerRateException));
            higherRate.MustBeGreaterThanOrEqualTo(0m, (_, __) =>
                new DomainException(Message.BelowZeroUpperRateException));
            lowerRate.MustBeLessThanOrEqualTo(higherRate, (_, __) =>
                new DomainException(Message.LowerRateAboveUpperException));

            LowerProductionLimit = lowerProductionLimit;
            UpperProductionLimit = upperProductionLimit;
            LowerRate = lowerRate;
            HigherRate = higherRate;
        }
    }
}