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
        public MonthlyAverageElectricEnergyProductionPrice MonthlyAverageElectricEnergyProductionPrice { get; private set; }
        public YearlyAverageElectricEnergyProductionPrice YearlyAverageElectricEnergyProductionPrice { get; private set; }

        protected CogenerationTariff() { }

        protected CogenerationTariff(
            NaturalGasSellingPrice naturalGasSellingPrice,
            YearlyAverageElectricEnergyProductionPrice yearlyAverageElectricEnergyProductionPrice,
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
                  identityFactory)
        {
            NaturalGasSellingPrice = naturalGasSellingPrice;
            YearlyAverageElectricEnergyProductionPrice = yearlyAverageElectricEnergyProductionPrice;
        }

        public CogenerationTariff CreateNewWith(
            ICogenerationParameterService cogenerationParameterService,
            YearlyAverageElectricEnergyProductionPrice yearlyAverageElectricEnergyProductionPrice,
            NaturalGasSellingPrice naturalGasSellingPrice,
            IIdentityFactory<Guid> identityFactory)
        {
            cogenerationParameterService.MustNotBeNull(message: SepsMessage.EntityNotSet(nameof(cogenerationParameterService)));
            yearlyAverageElectricEnergyProductionPrice.MustNotBeNull(message: SepsMessage.EntityNotSet(nameof(yearlyAverageElectricEnergyProductionPrice)));
            yearlyAverageElectricEnergyProductionPrice.IsActive().MustBe(true, message: SepsMessage.InactiveException(nameof(yearlyAverageElectricEnergyProductionPrice)));
            naturalGasSellingPrice.MustNotBeNull(message: SepsMessage.EntityNotSet(nameof(naturalGasSellingPrice)));
            naturalGasSellingPrice.IsActive().MustBe(true, message: SepsMessage.InactiveException(nameof(naturalGasSellingPrice)));

            var cogenerationParameter = CalculateCogenerationParameter(
                cogenerationParameterService, yearlyAverageElectricEnergyProductionPrice, naturalGasSellingPrice);

            SetInactive(naturalGasSellingPrice.Active.Since);

            return new CogenerationTariff
            (
                naturalGasSellingPrice,
                yearlyAverageElectricEnergyProductionPrice,
                LowerProductionLimit,
                UpperProductionLimit,
                Math.Round(cogenerationParameter * LowerRate, 4, MidpointRounding.AwayFromZero),
                Math.Round(cogenerationParameter * HigherRate, 4, MidpointRounding.AwayFromZero),
                ProjectTypeId,
                identityFactory
            );
        }

        public void NaturalGasSellingPriceCorrection(
            ICogenerationParameterService cogenerationParameterService,
            YearlyAverageElectricEnergyProductionPrice yearlyAverageElectricEnergyProductionPrice,
            NaturalGasSellingPrice correctedNgsp,
            CogenerationTariff previousCgn)
        {
            var cogenerationParameter = CalculateCogenerationParameter(
                cogenerationParameterService, yearlyAverageElectricEnergyProductionPrice, correctedNgsp);

            LowerRate = cogenerationParameter * previousCgn.LowerRate;
            HigherRate = cogenerationParameter * previousCgn.HigherRate;
            CorrectActiveSince(correctedNgsp.Active.Since);
            previousCgn.CorrectActiveUntil(correctedNgsp.Active.Since);
        }

        private decimal CalculateCogenerationParameter(
            ICogenerationParameterService cogenerationParameterService,
            YearlyAverageElectricEnergyProductionPrice yaep,
            NaturalGasSellingPrice ngsp) =>
            cogenerationParameterService.Calculate(yaep, ngsp);
    }
}