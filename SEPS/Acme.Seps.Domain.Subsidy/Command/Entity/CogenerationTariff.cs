using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Infrastructure;
using Acme.Seps.Domain.Subsidy.Command.DomainService;
using Acme.Seps.Domain.Subsidy.Command.Infrastructure;
using Light.GuardClauses;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Subsidy.Command.Entity
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
                  naturalGasSellingPrice.Active.Since,
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

            SetInactive(naturalGasSellingPrice.Active.Since);

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
            CorrectActiveSince(correctedNgsp.Active.Since);
            previousCgn.CorrectActiveUntil(correctedNgsp.Active.Since);
        }

        private decimal CalculateCogenerationParameter(
            ICogenerationParameterService cogenerationParameterService,
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            NaturalGasSellingPrice gsp) =>
            cogenerationParameterService.GetFrom(yearsNaturalGasSellingPrices, gsp);
    }
}