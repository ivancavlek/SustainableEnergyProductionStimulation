using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class CurrentActiveMonthlyEconometricIndexSpecification<TMonthlyEconometicIndex>
        : BaseSpecification<TMonthlyEconometicIndex>
        where TMonthlyEconometicIndex : MonthlyEconometricIndex<TMonthlyEconometicIndex>
    {
        public override Expression<Func<TMonthlyEconometicIndex, bool>> ToExpression() =>
            mei => !mei.MonthlyPeriod.ValidTill.HasValue;
    }
}