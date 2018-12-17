using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.UseCases.Subsidy.Command.Entity
{
    public class ConsumerPriceIndex : YearlyEconometricIndex<ConsumerPriceIndex>
    {
        protected override int DecimalPlaces => 4;

        protected ConsumerPriceIndex() { }

        internal protected ConsumerPriceIndex(
            decimal amount, string remark, DateTimeOffset since, IIdentityFactory<Guid> identityFactory)
            : base(amount, remark, since, identityFactory) { }
    }
}