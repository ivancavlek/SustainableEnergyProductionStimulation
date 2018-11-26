using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using Light.GuardClauses;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Subsidy.Entity
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
            cogenerationParameterService.MustNotBeNull(message: SubsidyMessages.CogenerationParameterServiceException);
            naturalGasSellingPrice.MustNotBeNull(message: SubsidyMessages.NaturalGasSellingPriceNotSetException);

            var cogenerationParameter = CalculateCogenerationParameter(
                cogenerationParameterService, yearsNaturalGasSellingPrices, naturalGasSellingPrice);

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

        public void GspCorrection(
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            ICogenerationParameterService cogenerationParameterService,
            NaturalGasSellingPrice gsp,
            CogenerationTariff previousCgn)
        {
            var cogenerationParameter = CalculateCogenerationParameter(
                cogenerationParameterService, yearsNaturalGasSellingPrices, gsp);

            LowerRate = cogenerationParameter * previousCgn.LowerRate;
            HigherRate = cogenerationParameter * previousCgn.HigherRate;
            Period = new Period(new MonthlyPeriodFactory(gsp.Period.ValidFrom));
        }

        private decimal CalculateCogenerationParameter(
            ICogenerationParameterService cogenerationParameterService,
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            NaturalGasSellingPrice gsp) =>
            cogenerationParameterService.GetFrom(yearsNaturalGasSellingPrices, gsp);
    }
}