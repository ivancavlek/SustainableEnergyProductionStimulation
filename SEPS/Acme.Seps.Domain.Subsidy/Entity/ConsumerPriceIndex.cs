using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public class ConsumerPriceIndex : YearlyEconometricIndex<ConsumerPriceIndex>
    {
        protected ConsumerPriceIndex() { }

        internal protected ConsumerPriceIndex(
            decimal amount, string remark, DateTimeOffset activeFrom, IIdentityFactory<Guid> identityFactory)
            : base(amount, 4, remark, activeFrom, identityFactory) { }
    }
}