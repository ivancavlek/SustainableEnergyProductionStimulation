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

        protected YearlyAverageElectricEnergyProductionPrice() { }

        public YearlyAverageElectricEnergyProductionPrice(
            IEnumerable<MonthlyAverageElectricEnergyProductionPrice> monthlyAverageElectricEnergyProductionPrices,
            decimal amount,
            string remark,
            YearlyPeriod lastYearlyPeriod,
            IIdentityFactory<Guid> guidIdentityFactory)
            : base(amount, 4, remark, lastYearlyPeriod, guidIdentityFactory)
        {
            monthlyAverageElectricEnergyProductionPrices.ToList().ForEach(m =>
                MonthlyAverageElectricEnergyProductionPrices.Add(m));
        }
    }
}