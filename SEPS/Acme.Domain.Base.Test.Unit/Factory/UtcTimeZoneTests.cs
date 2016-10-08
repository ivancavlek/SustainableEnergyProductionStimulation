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
            _timeZoneInfo = TimeZoneInfo.Local;
            _utcTimeZone = new UtcTimeZoneFactory(_timeZoneInfo);
        }

        public void RepositoryTimeIsUtcNowWithTimeZoneOffSet()
        {
            var result = _utcTimeZone.GetCurrentRepositoryDateTime();

            result.Should().BeCloseTo(DateTimeOffset.UtcNow, 5000); // due to VSTS build
            result.Should().HaveOffset(_timeZoneInfo.GetUtcOffset(DateTime.UtcNow));
        }

        public void DisplayTimeIsCurrentDateTimeWithoutOffset()
        {
            var result = _utcTimeZone.GetCurrentDisplayDateTime();

            result.Should().BeCloseTo(DateTime.Now, 5000); // due to VSTS build
        }

        public void CreatesRepositoryTimeWithDisplayTime()
        {
            var dateTimeNow = DateTime.Now;

            var result = _utcTimeZone.ToRepositoryDateTime(dateTimeNow);

            dateTimeNow = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Unspecified);

            result.Should().BeCloseTo(new DateTimeOffset(dateTimeNow, _timeZoneInfo.GetUtcOffset(dateTimeNow)).DateTime);
        }
    }
}