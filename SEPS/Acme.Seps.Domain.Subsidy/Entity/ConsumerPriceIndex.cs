using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public class ConsumerPriceIndex : YearlyEconometricIndex<ConsumerPriceIndex>
    {
        protected override int DecimalPlaces => 4;

        protected ConsumerPriceIndex() { }

        internal protected ConsumerPriceIndex(
            decimal amount, string remark, DateTimeOffset activeFrom, IIdentityFactory<Guid> identityFactory)
            : base(amount, remark, activeFrom, identityFactory) { }
    }
}