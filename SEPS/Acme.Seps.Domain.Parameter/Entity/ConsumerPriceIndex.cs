using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class ConsumerPriceIndex : YearlyEconometricIndex
    {
        protected ConsumerPriceIndex() { }

        public ConsumerPriceIndex(
            decimal amount,
            string remark,
            YearlyPeriod lastYearlyPeriod,
            IIdentityFactory<Guid> identityFactory)
            : base(amount, 4, remark, lastYearlyPeriod, identityFactory) { }
    }
}