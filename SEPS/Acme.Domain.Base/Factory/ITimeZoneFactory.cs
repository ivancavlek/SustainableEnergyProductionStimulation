using System;

namespace Acme.Domain.Base.Factory
{
    public interface ITimeZoneFactory
    {
        DateTime GetCurrentDisplayDateTime();

        DateTimeOffset GetCurrentRepositoryDateTime();

        DateTimeOffset ToRepositoryDateTime(DateTime displayTime);
    }
}