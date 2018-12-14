using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Infrastructure;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public class RenewableEnergySourceTariff : Tariff
    {
        public ConsumerPriceIndex ConsumerPriceIndex { get; private set; }

        protected RenewableEnergySourceTariff() { }

        protected RenewableEnergySourceTariff(
            ConsumerPriceIndex consumerPriceIndex,
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
                  consumerPriceIndex.Period.ActiveFrom,
                  identityFactory) =>
            ConsumerPriceIndex = consumerPriceIndex;

        public RenewableEnergySourceTariff CreateNewWith(
            ConsumerPriceIndex consumerPriceIndex, IIdentityFactory<Guid> identityFactory)
        {
            consumerPriceIndex.MustNotBeNull(message: SubsidyMessages.ConsumerPriceIndexNotSetException);
            consumerPriceIndex.IsActive().MustBe(true, message: SepsBaseMessage.InactiveException);

            Archive(consumerPriceIndex.Period.ActiveFrom);

            return new RenewableEnergySourceTariff
            (
                consumerPriceIndex,
                LowerProductionLimit,
                UpperProductionLimit,
                LowerRate,
                CalculateHigherRate(HigherRate, consumerPriceIndex.Amount),
                ProjectTypeId,
                identityFactory
            );
        }

        public void CpiCorrection(ConsumerPriceIndex cpi, RenewableEnergySourceTariff previousRes) =>
            HigherRate = CalculateHigherRate(previousRes.HigherRate, cpi.Amount);

        private decimal CalculateHigherRate(decimal higherRate, decimal cpiAmount) =>
            higherRate * CalculatedCpiRate(cpiAmount);

        private decimal CalculatedCpiRate(decimal cpiAmount) =>
            cpiAmount / 100M;
    }
}