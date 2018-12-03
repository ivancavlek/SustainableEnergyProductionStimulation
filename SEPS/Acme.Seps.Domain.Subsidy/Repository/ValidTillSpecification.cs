using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Subsidy.Repository
{
    public sealed class ValidTillSpecification<TAggregateRoot>
        : BaseSpecification<TAggregateRoot> where TAggregateRoot : SepsAggregateRoot
    {
        private readonly DateTimeOffset _validTill;

        public ValidTillSpecification(DateTimeOffset validTill)
        {
            _validTill = validTill;
        }

        public override Expression<Func<TAggregateRoot, bool>> ToExpression() =>
            res => res.Period.ValidTill == _validTill;
    }
}