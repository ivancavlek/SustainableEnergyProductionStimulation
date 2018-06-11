using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class RenewableEnergySourceTariff : Tariff
    {
        public ConsumerPriceIndex ConsumerPriceIndex { get; }

        protected RenewableEnergySourceTariff()
        {
        }

        protected RenewableEnergySourceTariff(
            ConsumerPriceIndex consumerPriceIndex,
            decimal lowerRate,
            decimal higherRate,
            IIdentityFactory<Guid> identityFactory)
            : base(lowerRate, higherRate, consumerPriceIndex.Period, identityFactory)
        {
            ConsumerPriceIndex = consumerPriceIndex;
        }

        public RenewableEnergySourceTariff CreateNewWith(
            ConsumerPriceIndex consumerPriceIndex, IIdentityFactory<Guid> identityFactory)
        {
            if (consumerPriceIndex == null)
                throw new ArgumentNullException(null, Infrastructure.Parameter.ConsumerPriceIndexNotSetException);

            CheckPeriodFor(consumerPriceIndex.Period.ValidFrom.Year);

            var calculatedHigherRate = HigherRate * CalculatedCpiRate(consumerPriceIndex.Amount);

            return new RenewableEnergySourceTariff
            (
                consumerPriceIndex,
                LowerRate,
                calculatedHigherRate,
                identityFactory
            );
        }

        private void CheckPeriodFor(int newYearPeriod)
        {
            if (!Period.ValidTill.Value.Year.Equals(newYearPeriod))
                throw new DomainException(Infrastructure.Parameter.RenewableEnergySourceTariffPeriodException);
        }

        private decimal CalculatedCpiRate(decimal cpiAmount) =>
            cpiAmount / 100M;
    }
}