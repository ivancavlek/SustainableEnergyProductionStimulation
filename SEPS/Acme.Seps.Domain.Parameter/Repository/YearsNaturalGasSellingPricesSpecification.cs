using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class YearsNaturalGasSellingPricesSpecification : ISpecification<NaturalGasSellingPrice>
    {
        private readonly int _year;

        public YearsNaturalGasSellingPricesSpecification(int year)
        {
            _year = year;
        }

        Expression<Func<NaturalGasSellingPrice, bool>> ISpecification<NaturalGasSellingPrice>.ToExpression() =>
            ngp => ngp.Period.IsWithin(new DateTime(_year, 1, 1), new DateTime(_year + 1, 1, 1));
    }
}