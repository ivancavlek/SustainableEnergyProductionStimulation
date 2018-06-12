using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Entity;
using System;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class YearsNaturalGasSellingPricesSpecification
        : ActiveWithinDatesSpecification<NaturalGasSellingPrice>
    {
        public YearsNaturalGasSellingPricesSpecification(int year)
            : base(new DateTime(year, 1, 1), new DateTime(year + 1, 1, 1)) { }
    }
}