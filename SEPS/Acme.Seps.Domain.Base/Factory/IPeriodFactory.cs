using System;

namespace Acme.Seps.Domain.Base.Factory
{
    public interface IPeriodFactory
    {
        DateTimeOffset ValidFrom { get; }
        DateTimeOffset? ValidTill { get; }
    }
}