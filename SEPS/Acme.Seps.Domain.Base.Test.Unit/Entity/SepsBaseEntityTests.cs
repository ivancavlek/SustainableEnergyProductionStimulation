using Acme.Domain.Base.Factory;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Entity;
using FluentAssertions;
using Moq;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Entity
{
    public class SepsBaseEntityTests
    {
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;
        private readonly Period _period;
        private readonly DateTimeOffset _repositoryTime;
        private readonly DateTimeOffset _repositoryTimePlusOneDay;

        public SepsBaseEntityTests()
        {
            _identityFactory = new Mock<IIdentityFactory<Guid>>();
            _repositoryTime = GetCorrectParameterDate(DateTimeOffset.UtcNow);
            _repositoryTimePlusOneDay = _repositoryTime.AddDays(1);
            _period = new Period(_repositoryTime);
        }

        public void SetsGivenPeriod()
        {
            var result = new DummySepsBaseEntity(_period, _identityFactory.Object);

            result.Period.ShouldBeEquivalentTo(_period);
        }

        public void ShowsIfEntityIsActiveOnGivenDate()
        {
            var result = new DummySepsBaseEntity(_period, _identityFactory.Object);

            result.IsActiveAt(_repositoryTimePlusOneDay);
        }

        public void SetsAnExpirationDateForTheEntity()
        {
            var result = new DummySepsBaseEntity(_period, _identityFactory.Object);
            result.SetExpirationDateTo(_repositoryTimePlusOneDay);

            result.Period.ValidFrom.Should().Be(_repositoryTime);
            result.Period.ValidTill.Should().HaveValue();
            result.Period.ValidTill.Should().Be(_repositoryTimePlusOneDay);
        }

        private DateTimeOffset GetCorrectParameterDate(DateTimeOffset date) =>
            date.Date.AddDays(1 - DateTime.Today.Day);
    }

    internal class DummySepsBaseEntity : SepsBaseEntity
    {
        public DummySepsBaseEntity(Period period, IIdentityFactory<Guid> identityFactory)
            : base(period, identityFactory) { }
    }
}