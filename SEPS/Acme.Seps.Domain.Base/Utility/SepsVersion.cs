using Acme.Seps.Domain.Base.ValueType;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Base.Utility
{
    public static class SepsVersion
    {
        private readonly static Dictionary<int, ActivePeriod> _applicationVersions;

        static SepsVersion() =>
            _applicationVersions = new Dictionary<int, ActivePeriod>()
            {
                [1] = new ActivePeriod(new DateTimeOffset(new DateTime(2007, 7, 1)))
            };

        public static DateTimeOffset InitialDate() =>
            _applicationVersions[1].Since;
    }
}