using System;
using System.Linq.Expressions;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;

namespace Acme.Seps.Domain.Base.Repository
{
    public class ActiveAtDateSpecification<TAggregateRoot>
        : BaseSpecification<TAggregateRoot> where TAggregateRoot : SepsBaseAggregate
    {
        private readonly DateTimeOffset _dateTime;

        public ActiveAtDateSpecification(DateTimeOffset dateTime)
        {
            _dateTime = dateTime;
        }

        public override Expression<Func<TAggregateRoot, bool>> ToExpression() =>
            art => art.Period.ValidFrom <= _dateTime
                && (!art.Period.ValidTill.HasValue || _dateTime < art.Period.ValidTill);
    }
}