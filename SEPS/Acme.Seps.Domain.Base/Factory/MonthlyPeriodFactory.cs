using Acme.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Utility;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Base.Infrastructure.Base;

namespace Acme.Seps.Domain.Base.Factory
{
    public sealed class MonthlyPeriodFactory : IPeriodFactory
    {
        public DateTimeOffset ValidFrom { get; }
        public DateTimeOffset? ValidTill { get; }

        public MonthlyPeriodFactory(DateTimeOffset dateFrom)
        {
            ValidFrom = dateFrom.ToFirstDayOfTheMonth();
        }

        public MonthlyPeriodFactory(DateTimeOffset dateFrom, DateTimeOffset dateTill)
        {
            dateTill.MustBeGreaterThanOrEqualTo(dateFrom, (_, __) =>
                new DomainException(Message.ValidTillGreaterThanValidFromException));

            ValidFrom = dateFrom.ToFirstDayOfTheMonth();
            ValidTill = dateTill.ToFirstDayOfTheMonth();
        }
    }
}