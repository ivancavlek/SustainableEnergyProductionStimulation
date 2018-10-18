using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.DomainService;
using Light.GuardClauses;
using System;
using System.Collections.Generic;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class CogenerationTariff : Tariff
    {
        public NaturalGasSellingPrice NaturalGasSellingPrice { get; private set; }

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
            yearsNaturalGasSellingPrices.MustNotBeNullOrEmpty(message: Message.YearsNaturalGasSellingPricesException);
            cogenerationParameterService.MustNotBeNull(message: Message.CogenerationParameterServiceException);
            naturalGasSellingPrice.MustNotBeNull(message: Message.NaturalGasSellingPriceNotSetException);

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