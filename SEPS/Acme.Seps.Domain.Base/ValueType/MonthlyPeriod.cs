using Acme.Domain.Base.Entity;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Utility;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Base.Infrastructure.Base;

namespace Acme.Seps.Domain.Base.ValueType
{
    public class MonthlyPeriod : ValueObject
    {
        public DateTimeOffset ValidFrom { get; private set; }
        public DateTimeOffset? ValidTill { get; private set; }

        protected MonthlyPeriod() { }

        public MonthlyPeriod(DateTimeOffset dateFrom)
        {
            dateFrom.Day.MustBe(1, (_, __) =>
                new DomainException(Message.DailyPeriodNotAllowedException));
            dateFrom.TimeOfDay.MustBe(TimeSpan.Zero, (_, __) =>
                new DomainException(Message.TimeOfDayPeriodNotAllowedException));

            ValidFrom = dateFrom.ToFirstDayOfTheMonth();
        }

        public MonthlyPeriod(DateTimeOffset dateFrom, DateTimeOffset dateTill)
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

        public MonthlyPeriod SetValidTill(DateTimeOffset validTill) =>
            new MonthlyPeriod(ValidFrom, validTill);

        public override string ToString() =>
            ValidTill.HasValue ? $"{ValidFrom:MM.yyyy.} - {ValidTill.Value:MM.yyyy.}" : $"{ValidFrom:MM.yyyy.} - ";
    }
}