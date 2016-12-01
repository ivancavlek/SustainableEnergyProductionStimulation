using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class CogenerationTariff : Tariff, IAggregateRoot
    {
        public NaturalGasSellingPrice NaturalGasSellingPrice { get; }

        protected CogenerationTariff()
        {
        }

        protected CogenerationTariff(
            NaturalGasSellingPrice naturalGasSellingPrice,
            decimal lowerRate,
            decimal higherRate,
            Period period,
            IIdentityFactory<Guid> identityFactory)
            : base(lowerRate, higherRate, period, identityFactory)
        {
            NaturalGasSellingPrice = naturalGasSellingPrice;
        }

        public CogenerationTariff CreateNewWith(
            decimal cogenerationParameter,
            Period newPeriod,
            NaturalGasSellingPrice naturalGasSellingPrice,
            IIdentityFactory<Guid> identityFactory)
        {
            if (naturalGasSellingPrice == null)
                throw new ArgumentNullException(null, Infrastructure.Parameter.NaturalGasSellingPriceNotSetException);
            if (!(Period.ValidFrom < (newPeriod.ValidTill ?? newPeriod.ValidFrom) &&
                (newPeriod.ValidTill ?? newPeriod.ValidFrom) < SystemTime.CurrentMonth()))
                throw new DomainException(Infrastructure.Parameter.ChpDateException);

            SetExpirationDateTo(newPeriod.ValidFrom);

            return new CogenerationTariff
            (
                naturalGasSellingPrice,
                cogenerationParameter * LowerRate,
                cogenerationParameter * HigherRate,
                newPeriod,
                identityFactory
            );
        }
    }
}