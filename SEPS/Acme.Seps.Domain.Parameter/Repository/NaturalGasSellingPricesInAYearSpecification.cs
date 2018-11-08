using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Parameter.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class NaturalGasSellingPricesInAYearSpecification : BaseSpecification<NaturalGasSellingPrice>
    {
        private readonly DateTimeOffset _previousYear;

        public NaturalGasSellingPricesInAYearSpecification()
        {
            _previousYear = SystemTime.CurrentYear().AddYears(-1);
        }

        public override Expression<Func<NaturalGasSellingPrice, bool>> ToExpression() =>
            nsp => nsp.MonthlyPeriod.ValidFrom.Year.Equals(_previousYear.Year);
    }
}