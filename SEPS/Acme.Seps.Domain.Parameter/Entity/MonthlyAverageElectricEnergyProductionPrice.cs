using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class MonthlyAverageElectricEnergyProductionPrice : MonthlyEconometricIndex
    {
        protected MonthlyAverageElectricEnergyProductionPrice() { }

        public MonthlyAverageElectricEnergyProductionPrice(
            decimal amount,
            string remark,
            MonthlyPeriod lastMonthlyPeriod,
            IIdentityFactory<Guid> guidIdentityFactory)
            : base(amount, 4, remark, lastMonthlyPeriod, guidIdentityFactory) { }
    }
}