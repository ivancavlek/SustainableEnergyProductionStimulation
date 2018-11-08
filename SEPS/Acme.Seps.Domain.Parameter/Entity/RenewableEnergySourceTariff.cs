using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class RenewableEnergySourceTariff : Tariff
    {
        public YearlyPeriod YearlyPeriod { get; private set; }
        public ConsumerPriceIndex ConsumerPriceIndex { get; private set; }

        protected RenewableEnergySourceTariff() { }

        protected RenewableEnergySourceTariff(
            ConsumerPriceIndex consumerPriceIndex,
            decimal lowerRate,
            decimal higherRate,
            IIdentityFactory<Guid> identityFactory)
            : base(lowerRate, higherRate, identityFactory)
        {
            YearlyPeriod = consumerPriceIndex.YearlyPeriod;
            ConsumerPriceIndex = consumerPriceIndex;
        }

        public RenewableEnergySourceTariff CreateNewWith(
            ConsumerPriceIndex consumerPriceIndex, IIdentityFactory<Guid> identityFactory)
        {
            consumerPriceIndex.MustNotBeNull(message: Message.ConsumerPriceIndexNotSetException);
            YearlyPeriod.ValidTill.Year.MustBe(consumerPriceIndex.YearlyPeriod.ValidFrom.Year, (_, __) =>
                new DomainException(Message.RenewableEnergySourceTariffPeriodException));

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