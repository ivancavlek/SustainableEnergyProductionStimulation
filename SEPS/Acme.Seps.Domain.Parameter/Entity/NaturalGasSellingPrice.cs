using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class NaturalGasSellingPrice : MonthlyEconometricIndex
    {
        protected NaturalGasSellingPrice()
        {
        }

        protected NaturalGasSellingPrice(
            decimal amount,
            string remark,
            MonthlyPeriod lastMonthlyPeriod,
            IIdentityFactory<Guid> identityFactory)
            : base(amount, 2, remark, lastMonthlyPeriod, identityFactory) { }

        public override MonthlyEconometricIndex CreateNew(
            decimal amount, string remark, DateTime validTill, IIdentityFactory<Guid> identityFactory)
        {
            SetExpirationDateTo(validTill);

            return new NaturalGasSellingPrice(
                amount, remark, (MonthlyPeriod)Period, identityFactory);
        }
    }
}