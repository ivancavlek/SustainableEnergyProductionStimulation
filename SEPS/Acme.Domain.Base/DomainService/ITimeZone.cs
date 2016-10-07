using System;

namespace Acme.Domain.Base.DomainService
{
    public interface ITimeZone
    {
        DateTime GetCurrentDisplayDateTime();

        DateTimeOffset GetCurrentRepositoryDateTime();

        DateTimeOffset ToRepositoryDateTime(DateTime displayTime);
    }
}