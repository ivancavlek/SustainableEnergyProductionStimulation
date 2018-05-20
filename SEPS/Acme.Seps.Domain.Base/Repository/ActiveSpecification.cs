using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Base.Repository
{
    public sealed class ActiveSpecification<TAggregateRoot>
        : BaseSpecification<TAggregateRoot>, ISpecification<TAggregateRoot>
        where TAggregateRoot : SepsBaseEntity, IAggregateRoot
    {
        Expression<Func<TAggregateRoot, bool>> ISpecification<TAggregateRoot>.ToExpression() =>
            aggregateRoot => aggregateRoot.IsActiveAt(DateTimeOffset.Now);
    }
}