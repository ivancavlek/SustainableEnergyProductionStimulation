using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class YearlyAverageElectricEnergyProductionPrice : YearlyEconometricIndex
    {
        public ICollection<MonthlyAverageElectricEnergyProductionPrice> MonthlyAverageElectricEnergyProductionPrices { get; }

        protected YearlyAverageElectricEnergyProductionPrice()
        {
        }

        public YearlyAverageElectricEnergyProductionPrice(
            IEnumerable<MonthlyAverageElectricEnergyProductionPrice> maeps,
            YearlyPeriod lastYearlyPeriod,
            IIdentityFactory<Guid> guidIdentityFactory)
            : base(
                  maeps.Sum(m => m.Amount) / maeps.Max(m => m.Period.ValidFrom.Month),
                  4,
                  Infrastructure.Parameter.AutomaticEntry,
                  lastYearlyPeriod,
                  guidIdentityFactory)
        {
            MonthlyAverageElectricEnergyProductionPrices = new List<MonthlyAverageElectricEnergyProductionPrice>();
            maeps.ToList().ForEach(m => MonthlyAverageElectricEnergyProductionPrices.Add(m));
        }
    }
}