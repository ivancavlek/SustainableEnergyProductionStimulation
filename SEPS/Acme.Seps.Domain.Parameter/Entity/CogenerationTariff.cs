using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.DomainService;
using System;
using System.Collections.Generic;
using System.Linq;

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
            : base(lowerRate, higherRate, period, identityFactory) =>
            NaturalGasSellingPrice = naturalGasSellingPrice;

        public CogenerationTariff CreateNewWith(
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            ICogenerationParameterService cogenerationParameterService,
            NaturalGasSellingPrice naturalGasSellingPrice,
            IIdentityFactory<Guid> identityFactory)
        {
            if (yearsNaturalGasSellingPrices == null || !yearsNaturalGasSellingPrices.Any())
                throw new ArgumentNullException(null, Infrastructure.Parameter.YearsNaturalGasSellingPricesException);
            if (cogenerationParameterService == null)
                throw new ArgumentNullException(null, Infrastructure.Parameter.CogenerationParameterServiceException);
            if (naturalGasSellingPrice == null)
                throw new ArgumentNullException(null, Infrastructure.Parameter.NaturalGasSellingPriceNotSetException);

            SetExpirationDateTo(naturalGasSellingPrice.Period.ValidFrom);

            var cogenerationParameter = cogenerationParameterService
                .GetFrom(yearsNaturalGasSellingPrices, naturalGasSellingPrice);

            return new CogenerationTariff
            (
                naturalGasSellingPrice,
                cogenerationParameter * LowerRate,
                cogenerationParameter * HigherRate,
                naturalGasSellingPrice.Period,
                identityFactory
            );
        }
    }
}