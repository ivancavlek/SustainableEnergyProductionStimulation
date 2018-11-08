using Acme.Domain.Base.Entity;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Utility;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Base.Infrastructure.Base;

namespace Acme.Seps.Domain.Base.ValueType
{
    public class YearlyPeriod : ValueObject
    {
        public DateTimeOffset ValidFrom { get; private set; }
        public DateTimeOffset ValidTill { get; private set; }

        protected YearlyPeriod() { }

        public YearlyPeriod(DateTimeOffset dateFrom, DateTimeOffset dateTill)
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

            ValidFrom = dateFrom.ToFirstMonthOfTheYear().ToFirstDayOfTheMonth();
            ValidTill = dateTill.ToFirstMonthOfTheYear().ToFirstDayOfTheMonth();

            (ValidTill.Year - ValidFrom.Year).MustBe(1, (_, __) =>
                new DomainException(Message.YearlyValueNotEqualYearException));
        }

        public override string ToString() =>
            $"{ValidFrom:yyyy}";
    }
}