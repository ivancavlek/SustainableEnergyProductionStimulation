using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Infrastructure;
using Light.GuardClauses;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Subsidy.Command.Repository
{
    public sealed class PreviousActiveSpecification<TAggregateRoot>
        : BaseSpecification<TAggregateRoot> where TAggregateRoot : SepsAggregateRoot
    {
        private readonly SepsAggregateRoot _activeAggregateRoot;

        public PreviousActiveSpecification(SepsAggregateRoot activeAggregateRoot)
        {
            _activeAggregateRoot = activeAggregateRoot ?? throw new ArgumentNullException(nameof(activeAggregateRoot));

            _activeAggregateRoot.IsActive().MustBe(true, message: SepsBaseMessage.InactiveException);
        }

        public override Expression<Func<TAggregateRoot, bool>> ToExpression() =>
            art => art.Active.Until == _activeAggregateRoot.Active.Since;
    }
}