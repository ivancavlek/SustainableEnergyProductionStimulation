using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Factory;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Base.ValueType
{
    public sealed class Period : ValueObject
    {
        public DateTimeOffset ValidFrom { get; private set; }
        public DateTimeOffset? ValidTill { get; private set; }

        private Period() { }

        public Period(IPeriodFactory periodFactory)
        {
            periodFactory.MustNotBeNull(nameof(periodFactory));

            ValidFrom = periodFactory.ValidFrom;
            ValidTill = periodFactory.ValidTill;
        }
    }
}