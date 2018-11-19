using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
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

        protected CogenerationTariff() { }

        protected CogenerationTariff(
            NaturalGasSellingPrice naturalGasSellingPrice,
            int lowerProductionLimit,
            int upperProductionLimit,
            decimal lowerRate,
            decimal higherRate,
            MonthlyPeriodFactory monthlyPeriodFactory,
            IIdentityFactory<Guid> identityFactory)
            : base(lowerProductionLimit,
                  upperProductionLimit,
                  lowerRate,
                  higherRate,
                  monthlyPeriodFactory,
                  identityFactory)
        {
            NaturalGasSellingPrice = naturalGasSellingPrice;
        }

        public CogenerationTariff CreateNewWith(
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            ICogenerationParameterService cogenerationParameterService,
            NaturalGasSellingPrice naturalGasSellingPrice,
            IIdentityFactory<Guid> identityFactory)
        {
            cogenerationParameterService.MustNotBeNull(message: Message.CogenerationParameterServiceException);
            naturalGasSellingPrice.MustNotBeNull(message: Message.NaturalGasSellingPriceNotSetException);

            var cogenerationParameter = cogenerationParameterService
                .GetFrom(yearsNaturalGasSellingPrices, naturalGasSellingPrice);

            Period = new Period(new MonthlyPeriodFactory(Period.ValidFrom, naturalGasSellingPrice.Period.ValidFrom));

            return new CogenerationTariff
            (
                naturalGasSellingPrice,
                LowerProductionLimit,
                UpperProductionLimit,
                cogenerationParameter * LowerRate,
                cogenerationParameter * HigherRate,
                new MonthlyPeriodFactory(naturalGasSellingPrice.Period.ValidFrom),
                identityFactory
            );
        }
    }
}