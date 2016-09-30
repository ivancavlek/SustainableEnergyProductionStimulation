using System;

namespace Acme.Seps.Domain.Base.DomainService
{
    public sealed class UtcTimeZone : ITimeZone
    {
        private readonly TimeZoneInfo _timeZoneInfo;

        public UtcTimeZone(TimeZoneInfo timeZoneInfo)
        {
            if (timeZoneInfo == null)
                throw new ArgumentNullException(nameof(timeZoneInfo));

            _timeZoneInfo = timeZoneInfo;
        }

        DateTime ITimeZone.GetCurrentDisplayDateTime() => GetCurrentRepositoryTime().DateTime;

        DateTimeOffset ITimeZone.GetCurrentRepositoryDateTime() => GetCurrentRepositoryTime();

        DateTimeOffset ITimeZone.ToRepositoryDateTime(DateTime displayDateTime) =>
            new DateTimeOffset(displayDateTime, _timeZoneInfo.GetUtcOffset(displayDateTime));

        private DateTimeOffset GetCurrentRepositoryTime() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZoneInfo);
    }
}