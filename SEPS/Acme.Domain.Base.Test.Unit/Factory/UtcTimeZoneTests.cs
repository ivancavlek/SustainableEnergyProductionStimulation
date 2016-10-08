using Acme.Domain.Base.Factory;
using FluentAssertions;
using System;

namespace Acme.Domain.Base.Test.Unit.Factory
{
    public class UtcTimeZoneTests
    {
        private readonly ITimeZoneFactory _utcTimeZone;

        private readonly TimeZoneInfo _timeZoneInfo;

        public UtcTimeZoneTests()
        {
            _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            _utcTimeZone = new UtcTimeZoneFactory(_timeZoneInfo);
        }

        public void RepositoryTimeIsUtcNowWithTimeZoneOffSet()
        {
            var result = _utcTimeZone.GetCurrentRepositoryDateTime();

            result.Should().BeCloseTo(DateTimeOffset.UtcNow);
            result.Should().HaveOffset(_timeZoneInfo.GetUtcOffset(DateTime.UtcNow));
        }

        public void DisplayTimeIsCurrentDateTimeWithoutOffset()
        {
            var result = _utcTimeZone.GetCurrentDisplayDateTime();

            var dateTimeNow = DateTime.Now;
            result.Should().BeCloseTo(new DateTimeOffset(dateTimeNow, _timeZoneInfo.GetUtcOffset(dateTimeNow)).DateTime);
        }

        public void CreatesRepositoryTimeWithDisplayTime()
        {
            var dateTimeNow = DateTime.Now;

            var result = _utcTimeZone.ToRepositoryDateTime(dateTimeNow);

            result.Should().BeCloseTo(new DateTimeOffset(dateTimeNow, _timeZoneInfo.GetUtcOffset(dateTimeNow)).DateTime);
        }
    }
}