using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class NaturalGasSellingPricesInAYearSpecification : BaseSpecification<NaturalGasSellingPrice>
    {
        private readonly int _year;

        public NaturalGasSellingPricesInAYearSpecification(int year)
        {
            _year = year;
        }

        public override Expression<Func<NaturalGasSellingPrice, bool>> ToExpression() =>
            nsp => nsp.Period.ValidFrom.Year.Equals(_year);
    }
}