using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public class ConsumerPriceIndex : YearlyEconometricIndex<ConsumerPriceIndex>
    {
        protected ConsumerPriceIndex() { }

        protected ConsumerPriceIndex(
            decimal amount,
            string remark,
            Period lastYearlyPeriod,
            IIdentityFactory<Guid> identityFactory)
            : base(amount, 4, remark, lastYearlyPeriod, identityFactory) { }

        public override ConsumerPriceIndex CreateNew(
            decimal amount, string remark, IIdentityFactory<Guid> identityFactory) =>
            new ConsumerPriceIndex(amount, remark, Period, identityFactory);
    }
}