using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Subsidy.Command.Entity
{
    public class NaturalGasSellingPrice : MonthlyEconometricIndex<NaturalGasSellingPrice>
    {
        protected override int DecimalPlaces => 2;

        protected NaturalGasSellingPrice() { }

        internal protected NaturalGasSellingPrice(
            decimal amount, string remark, DateTimeOffset since, IIdentityFactory<Guid> identityFactory)
            : base(amount, remark, since, identityFactory) { }
    }
}