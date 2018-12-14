using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public class MonthlyAverageElectricEnergyProductionPrice
        : MonthlyEconometricIndex<MonthlyAverageElectricEnergyProductionPrice>
    {
        protected override int DecimalPlaces => 4;

        protected MonthlyAverageElectricEnergyProductionPrice() { }

        internal protected MonthlyAverageElectricEnergyProductionPrice(
            decimal amount, string remark, DateTimeOffset activeFrom, IIdentityFactory<Guid> guidIdentityFactory)
            : base(amount, remark, activeFrom, guidIdentityFactory) { }
    }
}