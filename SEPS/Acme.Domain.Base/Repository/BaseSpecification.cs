using Acme.Domain.Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Acme.Domain.Base.Repository
{
    // eliminate in C# 8
    public abstract class BaseSpecification<TAggregateRoot>
    {
        public List<Expression<Func<TAggregateRoot, BaseEntity>>> Includes { get; } =
            new List<Expression<Func<TAggregateRoot, BaseEntity>>>();

        protected void AddInclude(Expression<Func<TAggregateRoot, BaseEntity>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}