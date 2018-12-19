using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public class YearlyAverageElectricEnergyProductionPrice
        : YearlyEconometricIndex<YearlyAverageElectricEnergyProductionPrice>
    {
        protected override int DecimalPlaces => 4;

        protected YearlyAverageElectricEnergyProductionPrice() { }

        internal protected YearlyAverageElectricEnergyProductionPrice(
            decimal amount, string remark, DateTimeOffset since, IIdentityFactory<Guid> guidIdentityFactory)
            : base(amount, remark, since, guidIdentityFactory) { }
    }
}