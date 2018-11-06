using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Factory;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Base.Repository
{
    public abstract class CurrentActiveYearlySpecification<TAggregateRoot>
        : BaseSpecification<TAggregateRoot> where TAggregateRoot : SepsBaseAggregate
    {
        private readonly DateTimeOffset _previousYear;

        public CurrentActiveYearlySpecification()
        {
            _previousYear = SystemTime.CurrentYear().AddYears(-1);
        }

        public override Expression<Func<TAggregateRoot, bool>> ToExpression() =>
            yei => yei.Period.ValidFrom <= _previousYear && _previousYear < yei.Period.ValidTill;
    }
}