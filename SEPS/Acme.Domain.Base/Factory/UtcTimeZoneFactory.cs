using System;

namespace Acme.Domain.Base.Factory
{
    public sealed class UtcTimeZoneFactory : ITimeZoneFactory
    {
        private readonly TimeZoneInfo _timeZoneInfo;

        public UtcTimeZoneFactory(TimeZoneInfo timeZoneInfo)
        {
            if (timeZoneInfo == null)
                throw new ArgumentNullException(nameof(timeZoneInfo));

            _timeZoneInfo = timeZoneInfo;
        }

        DateTime ITimeZoneFactory.GetCurrentDisplayDateTime() => GetCurrentRepositoryTime().DateTime;

        DateTimeOffset ITimeZoneFactory.GetCurrentRepositoryDateTime() => GetCurrentRepositoryTime();

        DateTimeOffset ITimeZoneFactory.ToRepositoryDateTime(DateTime displayDateTime)
        {
            displayDateTime = DateTime.SpecifyKind(displayDateTime, DateTimeKind.Unspecified);
            return new DateTimeOffset(displayDateTime, _timeZoneInfo.GetUtcOffset(displayDateTime));
        }

        private DateTimeOffset GetCurrentRepositoryTime() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZoneInfo);
    }
}