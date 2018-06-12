using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Base.Repository
{
    public class ActiveWithinDatesSpecification<TAggregateRoot>
        : BaseSpecification<TAggregateRoot> where TAggregateRoot : SepsBaseAggregate
    {
        private readonly DateTimeOffset _dateFrom;
        private readonly DateTimeOffset? _dateTill;

        public ActiveWithinDatesSpecification(DateTimeOffset dateFrom, DateTimeOffset? dateTill)
        {
            _dateFrom = dateFrom;
            _dateTill = dateTill;
        }

        public override Expression<Func<TAggregateRoot, bool>> ToExpression() =>
            art => ((!art.Period.ValidTill.HasValue) || (!art.Period.ValidTill.HasValue && !_dateTill.HasValue))
                || art.Period.ValidFrom <= _dateFrom && _dateTill <= art.Period.ValidTill.Value;
    }
}