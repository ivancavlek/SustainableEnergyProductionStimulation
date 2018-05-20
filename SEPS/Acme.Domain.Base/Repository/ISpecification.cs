using Acme.Domain.Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Acme.Domain.Base.Repository
{
    public interface ISpecification<TAggregateRoot> where TAggregateRoot : BaseEntity, IAggregateRoot
    {
        List<Expression<Func<TAggregateRoot, BaseEntity>>> Includes { get; }

        Expression<Func<TAggregateRoot, bool>> ToExpression();
    }
}