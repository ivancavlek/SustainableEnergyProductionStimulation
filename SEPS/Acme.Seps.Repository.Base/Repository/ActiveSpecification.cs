using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Repository;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Repository.Base.Repository
{
    public class ActiveSpecification<TAggregateRoot> : IActiveSpecification<TAggregateRoot>
        where TAggregateRoot : SepsBaseEntity, IAggregateRoot
    {
        Expression<Func<TAggregateRoot, bool>> ISpecification<TAggregateRoot>.ToExpression() =>
            aggregateRoot => aggregateRoot.IsActiveAt(DateTimeOffset.Now);
    }
}