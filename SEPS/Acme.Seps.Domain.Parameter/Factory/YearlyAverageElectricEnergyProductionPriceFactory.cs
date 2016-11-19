using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.Factory
{
    public class YearlyAverageElectricEnergyProductionPriceFactory : IYearlyAverageElectricEnergyProductionPriceFactory
    {
        private readonly YearlyPeriod _period;
        private readonly IEnumerable<MonthlyAverageElectricEnergyProductionPrice> _maeps;
        private readonly IIdentityFactory<Guid> _guidIdentityFactory;
        private readonly DateTime _initialPeriod;

        public YearlyAverageElectricEnergyProductionPriceFactory(
            YearlyPeriod lastYaepPeriod,
            IEnumerable<MonthlyAverageElectricEnergyProductionPrice> maeps,
            IIdentityFactory<Guid> guidIdentityFactory)
        {
            if (maeps == null || !maeps.Any() || !maeps.All(maep => maep.IsActiveAt(SystemTime.CurrentMonth())))
                throw new DomainException(Infrastructure.Parameter.MaepForYearlyCalculationNotSetException);
            if (!lastYaepPeriod.ValidTill.HasValue)
                throw new DomainException(Infrastructure.Parameter.PeriodValidTillNotSetException);

            _initialPeriod = DateTime.Parse(Infrastructure.Parameter.InitialPeriod);

            if (_initialPeriod.AddYears(-1) > lastYaepPeriod.ValidTill.Value ||
                lastYaepPeriod.ValidTill.Value > SystemTime.CurrentYear())
                throw new DomainException(string.Format(
                    Infrastructure.Parameter.YaepDateException, _initialPeriod, SystemTime.CurrentYear()));

            _guidIdentityFactory = guidIdentityFactory;
            _maeps = maeps;
            _period = new YearlyPeriod(lastYaepPeriod.ValidTill.Value, lastYaepPeriod.ValidTill.Value.AddYears(1));
        }

        YearlyAverageElectricEnergyProductionPrice IYearlyAverageElectricEnergyProductionPriceFactory.CreateNew() =>
            new YearlyAverageElectricEnergyProductionPrice(
                CalculateMaeps(), CalculateAmount(), Infrastructure.Parameter.AutomaticEntry, _period, _guidIdentityFactory);

        private decimal CalculateAmount()
        {
            var amount = 0M;
            var monthsToCalculate = 12;

            for (int month = GetInitialPeriodMonth(); month <= monthsToCalculate; month++)
                amount += _maeps.Single(MaepIsWithinPeriod).Amount;

            return amount /= IsInitialPeriod() ? _initialPeriod.Month - 1 : monthsToCalculate;
        }

        // možda bi se logika mogla kombinirati sa donjim
        private IEnumerable<MonthlyAverageElectricEnergyProductionPrice> CalculateMaeps() =>
            _maeps.Where(maep => maep.Period.ValidFrom <= _period.ValidFrom &&
                _period.ValidFrom < (maep.Period.ValidTill.HasValue ?
                    (maep.Period.ValidFrom.Year.Equals(maep.Period.ValidTill.Value.Year) ?
                        maep.Period.ValidTill.Value.AddYears(1) : maep.Period.ValidTill.Value) :
                        (_period.ValidFrom.AddYears(1))))
                .ToList();

        private int GetInitialPeriodMonth() => IsInitialPeriod() ? _initialPeriod.Month : 1;

        private bool MaepIsWithinPeriod(MonthlyAverageElectricEnergyProductionPrice maep) =>
            maep.Period.ValidFrom <= _period.ValidFrom &&
            _period.ValidFrom < (maep.Period.ValidTill ?? _period.ValidFrom.AddYears(1));

        private bool IsInitialPeriod() =>
            _initialPeriod.Equals(_period);
    }
}