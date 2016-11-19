using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class CogenerationTariff : Tariff, IAggregateRoot
    {
        public NaturalGasSellingPrice NaturalGasSellingPrice { get; }
        public YearlyAverageElectricEnergyProductionPrice YearlyAverageElectricEnergyProductionPrice { get; }

        protected CogenerationTariff() { }

        protected CogenerationTariff(
            NaturalGasSellingPrice naturalGasSellingPrice,
            YearlyAverageElectricEnergyProductionPrice yearlyAverageElectricEnergyProductionPrice,
            decimal lowerRate,
            decimal higherRate,
            Period period,
            IIdentityFactory<Guid> identityFactory)
            : base(lowerRate, higherRate, period, identityFactory)
        {
            NaturalGasSellingPrice = naturalGasSellingPrice;
            YearlyAverageElectricEnergyProductionPrice = yearlyAverageElectricEnergyProductionPrice;
        }

        public CogenerationTariff CreateNewWith(
            decimal lastQuarterGasPriceFor2006,
            decimal maepPriceFor2006,
            NaturalGasSellingPrice naturalGasSellingPrice,
            YearlyAverageElectricEnergyProductionPrice yaep,
            IIdentityFactory<Guid> identityFactory)
        {
            CheckGuardCode(lastQuarterGasPriceFor2006, maepPriceFor2006, naturalGasSellingPrice, yaep);

            var newPeriod = GetNewPeriodFrom(naturalGasSellingPrice, yaep);

            CheckPeriod(newPeriod);

            var parameter = GetCalculatingParameterFrom(
                yaep.Amount, naturalGasSellingPrice.Amount, maepPriceFor2006, lastQuarterGasPriceFor2006);

            var newLowerRate = parameter * LowerRate;
            var newHigherRate = parameter * HigherRate;

            return new CogenerationTariff
            (
                naturalGasSellingPrice,
                yaep,
                newLowerRate,
                newHigherRate,
                newPeriod,
                identityFactory
            );
        }

        private static void CheckGuardCode(
            decimal lastQuarterGasPriceFor2006,
            decimal maepPriceFor2006,
            NaturalGasSellingPrice naturalGasSellingPrice,
            YearlyAverageElectricEnergyProductionPrice yaep)
        {
            if (lastQuarterGasPriceFor2006 <= 0)
                throw new DomainException(Infrastructure.Parameter.LastQuarterGasPriceFor2006Exception);
            if (maepPriceFor2006 <= 0)
                throw new DomainException(Infrastructure.Parameter.MaepPriceFor2006Exception);
            if (naturalGasSellingPrice == null)
                throw new ArgumentNullException(null, Infrastructure.Parameter.NaturalGasSellingPriceNotSetException);
            if (yaep == null)
                throw new ArgumentNullException(null, Infrastructure.Parameter.YaepNotSetException);
        }

        private static Period GetNewPeriodFrom(
            NaturalGasSellingPrice naturalGasSellingPrice,
            YearlyAverageElectricEnergyProductionPrice yaep) =>
            naturalGasSellingPrice.Period.ValidFrom < yaep.Period.ValidTill.Value ?
                yaep.Period :
                naturalGasSellingPrice.Period;

        private void CheckPeriod(Period newPeriod)
        {
            if (!(Period.ValidFrom < (newPeriod.ValidTill ?? newPeriod.ValidFrom) &&
                (newPeriod.ValidTill ?? newPeriod.ValidFrom) < SystemTime.CurrentMonth()))
                throw new DomainException(Infrastructure.Parameter.ChpDateException);
        }

        private decimal GetCalculatingParameterFrom(
            decimal yaepAmount,
            decimal naturalGasSellingPriceAmount,
            decimal maepPriceFor2006,
            decimal lastQuarterGasPriceFor2006)
        {
            var factor = 0.25M;

            var yaepRate = factor * (yaepAmount / maepPriceFor2006);
            var naturalGasSellingPriceRate =
                (1 - factor) * (naturalGasSellingPriceAmount / lastQuarterGasPriceFor2006);

            return yaepRate + naturalGasSellingPriceRate;
        }
    }
}