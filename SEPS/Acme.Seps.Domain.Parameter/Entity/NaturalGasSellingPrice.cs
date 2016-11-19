using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class NaturalGasSellingPrice : MonthlyEconometricIndex
    {
        protected NaturalGasSellingPrice() { }

        public NaturalGasSellingPrice(
            decimal amount,
            string remark,
            MonthlyPeriod lastMonthlyPeriod,
            IIdentityFactory<Guid> identityFactory)
            : base(amount, 2, remark, lastMonthlyPeriod, identityFactory) { }
    }
}