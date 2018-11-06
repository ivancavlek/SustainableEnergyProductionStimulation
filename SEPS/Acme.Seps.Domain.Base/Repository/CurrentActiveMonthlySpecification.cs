using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Base.Repository
{
    public abstract class CurrentActiveMonthlySpecification<TAggregateRoot>
        : BaseSpecification<TAggregateRoot> where TAggregateRoot : SepsBaseAggregate
    {
        public override Expression<Func<TAggregateRoot, bool>> ToExpression() =>
            mei => !mei.Period.ValidTill.HasValue;
    }
}