using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public class NaturalGasSellingPrice : MonthlyEconometricIndex<NaturalGasSellingPrice>
    {
        protected NaturalGasSellingPrice() { }

        internal protected NaturalGasSellingPrice(
            decimal amount, string remark, DateTimeOffset activeFrom, IIdentityFactory<Guid> identityFactory)
            : base(amount, 2, remark, activeFrom, identityFactory) { }
    }
}