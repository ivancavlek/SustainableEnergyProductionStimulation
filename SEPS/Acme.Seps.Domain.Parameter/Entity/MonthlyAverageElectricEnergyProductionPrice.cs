using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class MonthlyAverageElectricEnergyProductionPrice : MonthlyEconometricIndex
    {
        protected MonthlyAverageElectricEnergyProductionPrice()
        {
        }

        protected MonthlyAverageElectricEnergyProductionPrice(
            decimal amount,
            string remark,
            MonthlyPeriod lastMonthlyPeriod,
            IIdentityFactory<Guid> guidIdentityFactory)
            : base(amount, 4, remark, lastMonthlyPeriod, guidIdentityFactory) { }

        public override MonthlyEconometricIndex CreateNew(
            decimal amount, string remark, int month, int year, IIdentityFactory<Guid> identityFactory)
        {
            SetExpirationDateTo(new DateTime(year, month, 1));

            return new MonthlyAverageElectricEnergyProductionPrice(
                amount, remark, (MonthlyPeriod)Period, identityFactory);
        }
    }
}