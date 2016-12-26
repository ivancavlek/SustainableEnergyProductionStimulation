using Acme.Domain.Base.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Domain.Base.Repository
{
    public interface ISpecification<TAggregateRoot> where TAggregateRoot : BaseEntity, IAggregateRoot
    {
        Expression<Func<TAggregateRoot, bool>> ToExpression();
    }
}