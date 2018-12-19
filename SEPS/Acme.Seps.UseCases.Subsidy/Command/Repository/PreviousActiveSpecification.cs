using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Text;
using Light.GuardClauses;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.UseCases.Subsidy.Command.Repository
{
    public sealed class PreviousActiveSpecification<TAggregateRoot>
        : BaseSpecification<TAggregateRoot> where TAggregateRoot : SepsAggregateRoot
    {
        private readonly SepsAggregateRoot _activeAggregateRoot;

        public PreviousActiveSpecification(SepsAggregateRoot activeAggregateRoot)
        {
            _activeAggregateRoot = activeAggregateRoot ?? throw new ArgumentNullException(nameof(activeAggregateRoot));

            _activeAggregateRoot.IsActive().MustBe(true, message: SepsMessage.InactiveException(activeAggregateRoot.GetType().Name));
        }

        public override Expression<Func<TAggregateRoot, bool>> ToExpression() =>
            art => art.Active.Until == _activeAggregateRoot.Active.Since;
    }
}