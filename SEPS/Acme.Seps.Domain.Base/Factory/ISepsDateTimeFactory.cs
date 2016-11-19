using Acme.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Base.Factory
{
    interface ISepsDateTimeFactory
    {
        DateTime CurrentDate { get; }
        int CurrentYear { get; }
        Period ToMonthlyPeriod(Period period);
        Period ToYearlyPeriod(Period period);
    }
}