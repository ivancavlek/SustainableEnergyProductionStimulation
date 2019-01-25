using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Text;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public class CogenerationTariff : Tariff
    {
        public NaturalGasSellingPrice NaturalGasSellingPrice { get; private set; }
        public AverageElectricEnergyProductionPrice AverageElectricEnergyProductionPrice { get; private set; }

        protected CogenerationTariff() { }

        protected CogenerationTariff(
            NaturalGasSellingPrice naturalGasSellingPrice,
            AverageElectricEnergyProductionPrice averageElectricEnergyProductionPrice,
            decimal? lowerProductionLimit,
            decimal? upperProductionLimit,
            decimal lowerRate,
            decimal higherRate,
            DateTimeOffset activeSince,
            Guid projectTypeId,
            IIdentityFactory<Guid> identityFactory)
            : base(lowerProductionLimit,
                  upperProductionLimit,
                  lowerRate,
                  higherRate,
                  projectTypeId,
                  activeSince,
                  identityFactory)
        {
            NaturalGasSellingPrice = naturalGasSellingPrice;
            AverageElectricEnergyProductionPrice = averageElectricEnergyProductionPrice;
        }

        public CogenerationTariff CreateNewWith(
            ICogenerationParameterService cogenerationParameterService,
            AverageElectricEnergyProductionPrice averageElectricEnergyProductionPrice,
            NaturalGasSellingPrice naturalGasSellingPrice,
            DateTimeOffset activeSince,
            IIdentityFactory<Guid> identityFactory)
        {
            cogenerationParameterService.MustNotBeNull(message: SepsMessage.EntityNotSet(nameof(cogenerationParameterService)));
            averageElectricEnergyProductionPrice.MustNotBeNull(message: SepsMessage.EntityNotSet(nameof(averageElectricEnergyProductionPrice)));
            averageElectricEnergyProductionPrice.IsActive().MustBe(true, message: SepsMessage.InactiveException(nameof(averageElectricEnergyProductionPrice)));
            naturalGasSellingPrice.MustNotBeNull(message: SepsMessage.EntityNotSet(nameof(naturalGasSellingPrice)));
            naturalGasSellingPrice.IsActive().MustBe(true, message: SepsMessage.InactiveException(nameof(naturalGasSellingPrice)));

            var cogenerationParameter = CalculateCogenerationParameter(
                cogenerationParameterService, averageElectricEnergyProductionPrice, naturalGasSellingPrice);

            SetInactive(activeSince);

            return new CogenerationTariff
            (
                naturalGasSellingPrice,
                averageElectricEnergyProductionPrice,
                LowerProductionLimit,
                UpperProductionLimit,
                Math.Round(cogenerationParameter * LowerRate, 4, MidpointRounding.AwayFromZero),
                Math.Round(cogenerationParameter * HigherRate, 4, MidpointRounding.AwayFromZero),
                activeSince,
                ProjectTypeId,
                identityFactory
            );
        }

        public void NaturalGasSellingPriceCorrection(
            ICogenerationParameterService cogenerationParameterService,
            AverageElectricEnergyProductionPrice averageElectricEnergyProductionPrice,
            NaturalGasSellingPrice correctedNgsp,
            CogenerationTariff previousCgn)
        {
            var cogenerationParameter = CalculateCogenerationParameter(
                cogenerationParameterService, averageElectricEnergyProductionPrice, correctedNgsp);

            LowerRate = cogenerationParameter * previousCgn.LowerRate;
            HigherRate = cogenerationParameter * previousCgn.HigherRate;
            CorrectActiveSince(correctedNgsp.Active.Since);
            previousCgn.CorrectActiveUntil(correctedNgsp.Active.Since);
        }

        private decimal CalculateCogenerationParameter(
            ICogenerationParameterService cogenerationParameterService,
            AverageElectricEnergyProductionPrice averageElectricEnergyProductionPrice,
            NaturalGasSellingPrice ngsp) =>
            cogenerationParameterService.Calculate(averageElectricEnergyProductionPrice, ngsp);
    }
}