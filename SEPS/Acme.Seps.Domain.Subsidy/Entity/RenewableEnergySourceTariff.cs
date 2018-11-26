using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
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
            int lowerProductionLimit,
            int upperProductionLimit,
            decimal lowerRate,
            decimal higherRate,
            Guid projectTypeId,
            IPeriodFactory periodFactory,
            IIdentityFactory<Guid> identityFactory)
            : base(lowerProductionLimit,
                  upperProductionLimit,
                  lowerRate,
                  higherRate,
                  projectTypeId,
                  periodFactory,
                  identityFactory)
        {
            ConsumerPriceIndex = consumerPriceIndex;
        }

        public RenewableEnergySourceTariff CreateNewWith(
            ConsumerPriceIndex consumerPriceIndex, IIdentityFactory<Guid> identityFactory)
        {
            consumerPriceIndex.MustNotBeNull(message: SubsidyMessages.ConsumerPriceIndexNotSetException);
            Period.ValidTill.Value.Year.MustBe(consumerPriceIndex.Period.ValidFrom.Year, (_, __) =>
                new DomainException(SubsidyMessages.RenewableEnergySourceTariffPeriodException));

            return new RenewableEnergySourceTariff
            (
                consumerPriceIndex,
                LowerProductionLimit,
                UpperProductionLimit,
                LowerRate,
                CalculateHigherRate(HigherRate, CalculatedCpiRate(consumerPriceIndex.Amount)),
                ProjectTypeId,
                new YearlyPeriodFactory(consumerPriceIndex.Period.ValidFrom, consumerPriceIndex.Period.ValidTill.Value),
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