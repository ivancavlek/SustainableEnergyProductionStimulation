using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.UseCases.Subsidy.Command.Entity
{
    public class MonthlyAverageElectricEnergyProductionPrice
        : MonthlyEconometricIndex<MonthlyAverageElectricEnergyProductionPrice>
    {
        protected override int DecimalPlaces => 4;

        protected MonthlyAverageElectricEnergyProductionPrice() { }

        internal protected MonthlyAverageElectricEnergyProductionPrice(
            decimal amount, string remark, DateTimeOffset since, IIdentityFactory<Guid> guidIdentityFactory)
            : base(amount, remark, since, guidIdentityFactory) { }
    }
}