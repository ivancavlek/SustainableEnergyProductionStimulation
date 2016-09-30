using System;

namespace Acme.Seps.Domain.Base.DomainService
{
    public interface ITimeZone
    {
        DateTime GetCurrentDisplayDateTime();

        DateTimeOffset GetCurrentRepositoryDateTime();

        DateTimeOffset ToRepositoryDateTime(DateTime displayTime);
    }
}