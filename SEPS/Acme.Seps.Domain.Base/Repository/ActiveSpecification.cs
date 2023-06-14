using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Base.Repository;

public sealed class ActiveSpecification<TAggregateRoot>
    : BaseSpecification<TAggregateRoot> where TAggregateRoot : SepsAggregateRoot
{
    public override Expression<Func<TAggregateRoot, bool>> ToExpression() =>
        sar => !sar.Active.Until.HasValue;
}