using Acme.Seps.Domain.Base.ValueType;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Base.Utility
{
    public static class SepsVersion
    {
        private readonly static Dictionary<int, Period> _applicationVersions;

        static SepsVersion() =>
            _applicationVersions = new Dictionary<int, Period>()
            {
                [1] = new Period(new DateTimeOffset(new DateTime(2007, 7, 1)))
            };

        public static DateTimeOffset InitialDate() =>
            _applicationVersions[1].ActiveFrom;
    }
}