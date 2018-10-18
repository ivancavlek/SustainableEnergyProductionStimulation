using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class RenewableEnergySourceTariff : Tariff
    {
        public ConsumerPriceIndex ConsumerPriceIndex { get; private set; }

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
            consumerPriceIndex.MustNotBeNull(message: Message.ConsumerPriceIndexNotSetException);
            Period.ValidTill.Value.Year.MustBe(consumerPriceIndex.Period.ValidFrom.Year, (x, y) => new DomainException(Message.RenewableEnergySourceTariffPeriodException));

            var calculatedHigherRate = HigherRate * CalculatedCpiRate(consumerPriceIndex.Amount);

            return new RenewableEnergySourceTariff
            (
                consumerPriceIndex,
                LowerRate,
                calculatedHigherRate,
                identityFactory
            );
        }

        private decimal CalculatedCpiRate(decimal cpiAmount) =>
            cpiAmount / 100M;
    }
}