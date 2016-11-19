using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Entity;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public abstract class Tariff : SepsBaseEntity, IAggregateRoot
    {
        public int LowerProductionLimit { get; }
        public int UpperProductionLimit { get; }
        public decimal LowerRate { get; }
        public decimal HigherRate { get; }

        protected Tariff() { }

        protected Tariff(
            decimal lowerRate,
            decimal higherRate,
            Period period,
            IIdentityFactory<Guid> identityFactory) : base(period, identityFactory)
        {
            if (lowerRate < 0)
                throw new DomainException(Infrastructure.Parameter.BelowZeroLowerRateException);
            if (higherRate < 0)
                throw new DomainException(Infrastructure.Parameter.BelowZeroUpperRateException);
            if (lowerRate > higherRate)
                throw new DomainException(Infrastructure.Parameter.LowerRateAboveUpperException);

            LowerRate = lowerRate;
            HigherRate = higherRate;
        }
    }
}