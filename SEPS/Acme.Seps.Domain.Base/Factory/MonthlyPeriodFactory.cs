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
            dateFrom.Day.MustBe(1, (_, __) =>
                new DomainException(Message.DailyPeriodNotAllowedException));
            dateFrom.TimeOfDay.MustBe(TimeSpan.Zero, (_, __) =>
                new DomainException(Message.TimeOfDayPeriodNotAllowedException));

            ValidFrom = dateFrom.ToFirstDayOfTheMonth();
        }

        public MonthlyPeriodFactory(DateTimeOffset dateFrom, DateTimeOffset dateTill)
        {
            dateFrom.Day.MustBe(1, (_, __) =>
                new DomainException(Message.DailyPeriodNotAllowedException));
            dateFrom.TimeOfDay.MustBe(TimeSpan.Zero, (_, __) =>
                new DomainException(Message.TimeOfDayPeriodNotAllowedException));
            dateTill.Day.MustBe(1, (_, __) =>
                new DomainException(Message.DailyPeriodNotAllowedException));
            dateTill.TimeOfDay.MustBe(TimeSpan.Zero, (_, __) =>
                new DomainException(Message.TimeOfDayPeriodNotAllowedException));
            dateTill.MustBeGreaterThanOrEqualTo(dateFrom, (_, __) =>
                new DomainException(Message.ValidTillGreaterThanValidFromException));

            ValidFrom = dateFrom.ToFirstDayOfTheMonth();
            ValidTill = dateTill.ToFirstDayOfTheMonth();
        }
    }
}