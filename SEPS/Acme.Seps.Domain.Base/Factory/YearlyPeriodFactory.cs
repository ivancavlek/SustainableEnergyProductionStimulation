﻿using Acme.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Utility;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Base.Infrastructure.Base;

namespace Acme.Seps.Domain.Base.Factory
{
    public sealed class YearlyPeriodFactory : IPeriodFactory
    {
        public DateTimeOffset ValidFrom { get; }
        public DateTimeOffset? ValidTill { get; }

        private YearlyPeriodFactory() { }

        public YearlyPeriodFactory(DateTimeOffset dateFrom, DateTimeOffset dateTill)
        {
            dateTill.MustBeGreaterThanOrEqualTo(dateFrom, (_, __) =>
                new DomainException(Message.ValidTillGreaterThanValidFromException));

            ValidFrom = dateFrom.ToFirstMonthOfTheYear().ToFirstDayOfTheMonth();
            ValidTill = dateTill.ToFirstMonthOfTheYear().ToFirstDayOfTheMonth();

            (ValidTill.Value.Year - ValidFrom.Year).MustBe(1, (_, __) =>
                new DomainException(Message.YearlyValueNotEqualYearException));
        }
    }
}