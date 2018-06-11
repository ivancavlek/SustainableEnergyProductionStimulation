using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class YearsNaturalGasSellingPricesSpecification
        : BaseSpecification<NaturalGasSellingPrice>
    {
        private readonly int _year;

        public YearsNaturalGasSellingPricesSpecification(int year) =>
            _year = year;

        public override Expression<Func<NaturalGasSellingPrice, bool>> ToExpression() =>
            ngp => ngp.IsWithin(new DateTime(_year, 1, 1), new DateTime(_year + 1, 1, 1));
    }
}