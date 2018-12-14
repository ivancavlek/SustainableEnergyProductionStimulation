using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Infrastructure;
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
            decimal? lowerProductionLimit,
            decimal? upperProductionLimit,
            decimal lowerRate,
            decimal higherRate,
            Guid projectTypeId,
            IIdentityFactory<Guid> identityFactory)
            : base(lowerProductionLimit,
                  upperProductionLimit,
                  lowerRate,
                  higherRate,
                  projectTypeId,
                  naturalGasSellingPrice.Period.ActiveFrom,
                  identityFactory) =>
            NaturalGasSellingPrice = naturalGasSellingPrice;

        public CogenerationTariff CreateNewWith(
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            ICogenerationParameterService cogenerationParameterService,
            NaturalGasSellingPrice naturalGasSellingPrice,
            IIdentityFactory<Guid> identityFactory)
        {
            cogenerationParameterService.MustNotBeNull(message: SubsidyMessages.CogenerationParameterServiceException);
            naturalGasSellingPrice.MustNotBeNull(message: SubsidyMessages.NaturalGasSellingPriceNotSetException);
            naturalGasSellingPrice.IsActive().MustBe(true, message: SepsBaseMessage.InactiveException);

            var cogenerationParameter = CalculateCogenerationParameter(
                cogenerationParameterService, yearsNaturalGasSellingPrices, naturalGasSellingPrice);

            Archive(naturalGasSellingPrice.Period.ActiveFrom);

            return new CogenerationTariff
            (
                naturalGasSellingPrice,
                LowerProductionLimit,
                UpperProductionLimit,
                cogenerationParameter * LowerRate,
                cogenerationParameter * HigherRate,
                ProjectTypeId,
                identityFactory
            );
        }

        public void NgspCorrection(
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            ICogenerationParameterService cogenerationParameterService,
            NaturalGasSellingPrice correctedNgsp,
            CogenerationTariff previousCgn)
        {
            var cogenerationParameter = CalculateCogenerationParameter(
                cogenerationParameterService, yearsNaturalGasSellingPrices, correctedNgsp);

            LowerRate = cogenerationParameter * previousCgn.LowerRate;
            HigherRate = cogenerationParameter * previousCgn.HigherRate;
            CorrectActiveFrom(correctedNgsp.Period.ActiveFrom);
            previousCgn.CorrectActiveTill(correctedNgsp.Period.ActiveFrom);
        }

        private decimal CalculateCogenerationParameter(
            ICogenerationParameterService cogenerationParameterService,
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            NaturalGasSellingPrice gsp) =>
            cogenerationParameterService.GetFrom(yearsNaturalGasSellingPrices, gsp);
    }
}