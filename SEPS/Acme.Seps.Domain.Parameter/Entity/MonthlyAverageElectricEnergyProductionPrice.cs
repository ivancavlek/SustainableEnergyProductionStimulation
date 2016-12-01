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
            decimal amount, string remark, DateTime validTill, IIdentityFactory<Guid> identityFactory)
        {
            SetExpirationDateTo(validTill);

            return new MonthlyAverageElectricEnergyProductionPrice(
                amount, remark, (MonthlyPeriod)Period, identityFactory);
        }
    }
}