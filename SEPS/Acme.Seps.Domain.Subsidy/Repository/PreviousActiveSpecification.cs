using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Subsidy.Repository
{
    public sealed class PreviousActiveSpecification<TAggregateRoot>
        : BaseSpecification<TAggregateRoot> where TAggregateRoot : SepsAggregateRoot
    {
        private readonly DateTimeOffset _activeTill;

        public PreviousActiveSpecification(DateTimeOffset activeTill)
        {
            _activeTill = activeTill;
        }

        public override Expression<Func<TAggregateRoot, bool>> ToExpression() =>
            art => art.Active.Until == _activeTill;
    }
}