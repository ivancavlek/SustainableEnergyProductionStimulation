using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public class MonthlyAverageElectricEnergyProductionPrice
        : MonthlyEconometricIndex<MonthlyAverageElectricEnergyProductionPrice>
    {
        protected MonthlyAverageElectricEnergyProductionPrice() { }

        internal protected MonthlyAverageElectricEnergyProductionPrice(
            decimal amount, string remark, DateTimeOffset activeFrom, IIdentityFactory<Guid> guidIdentityFactory)
            : base(amount, 4, remark, activeFrom, guidIdentityFactory) { }
    }
}