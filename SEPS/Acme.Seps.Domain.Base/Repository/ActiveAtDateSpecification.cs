using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Factory;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Base.Repository
{
    public class ActiveAtDateSpecification<TAggregateRoot>
        : BaseSpecification<TAggregateRoot> where TAggregateRoot : SepsBaseAggregate
    {
        private readonly DateTimeOffset _dateTime;

        public ActiveAtDateSpecification()
        {
            _dateTime = SystemTime.CurrentDateTime();
        }

        public ActiveAtDateSpecification(DateTimeOffset dateTime)
        {
            _dateTime = dateTime;
        }

        public override Expression<Func<TAggregateRoot, bool>> ToExpression() =>
            art => art.Period.ValidFrom <= _dateTime
                && (!art.Period.ValidTill.HasValue || _dateTime < art.Period.ValidTill);
    }
}