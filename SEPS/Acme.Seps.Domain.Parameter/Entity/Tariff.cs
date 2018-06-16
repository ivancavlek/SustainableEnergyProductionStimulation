using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.ValueType;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public abstract class Tariff : SepsBaseAggregate
    {
        public int LowerProductionLimit { get; }
        public int UpperProductionLimit { get; }
        public decimal LowerRate { get; }
        public decimal HigherRate { get; }

        protected Tariff()
        {
        }

        protected Tariff(
            decimal lowerRate,
            decimal higherRate,
            Period period,
            IIdentityFactory<Guid> identityFactory) : base(period, identityFactory)
        {
            lowerRate.MustBeGreaterThanOrEqualTo(0m, exception: () => new DomainException(Message.BelowZeroLowerRateException));
            higherRate.MustBeGreaterThanOrEqualTo(0m, exception: () => new DomainException(Message.BelowZeroUpperRateException));
            lowerRate.MustBeLessThanOrEqualTo(higherRate, exception: () => new DomainException(Message.LowerRateAboveUpperException));

            LowerRate = lowerRate;
            HigherRate = higherRate;
        }
    }
}