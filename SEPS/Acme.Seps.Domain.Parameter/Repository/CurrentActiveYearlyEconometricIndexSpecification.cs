using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Parameter.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class CurrentActiveYearlyEconometricIndexSpecification<TYearlyEconometicIndex>
        : BaseSpecification<TYearlyEconometicIndex>
        where TYearlyEconometicIndex : YearlyEconometricIndex<TYearlyEconometicIndex>
    {
        private readonly DateTimeOffset _previousYear;

        public CurrentActiveYearlyEconometricIndexSpecification()
        {
            _previousYear = SystemTime.CurrentYear().AddYears(-1);
        }

        public override Expression<Func<TYearlyEconometicIndex, bool>> ToExpression() =>
            yei => yei.YearlyPeriod.ValidFrom <= _previousYear && _previousYear < yei.YearlyPeriod.ValidTill;
    }
}