using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.ValueType;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Entity
{
    public class SepsBaseAggregateTests
    {
        private readonly IIdentityFactory<Guid> _identityFactory;
        private readonly Period _period;
        private readonly DateTimeOffset _repositoryTime;
        private readonly DateTimeOffset _repositoryTimePlusOneDay;
        private readonly DateTimeOffset _repositoryTimeMinusOneDay;
        private readonly DateTimeOffset _repositoryTimePlusOneMonth;

        public SepsBaseAggregateTests()
        {
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _repositoryTime = GetCorrectParameterDate(DateTimeOffset.UtcNow);
            _period = new Period(_repositoryTime);

            _repositoryTimePlusOneDay = _repositoryTime.AddDays(1);
            _repositoryTimeMinusOneDay = _repositoryTime.AddDays(-1);
            _repositoryTimePlusOneDay = _repositoryTime.AddDays(1);
            _repositoryTimePlusOneMonth = _repositoryTime.AddMonths(1);
        }

        public void SetsGivenPeriod()
        {
            var result = new DummySepsBaseAggregate(_period, _identityFactory);

            result.Period.Should().BeEquivalentTo(_period);
        }

        public void SetsAnExpirationDateForTheEntity()
        {
            var result = new DummySepsBaseAggregate(_period, _identityFactory);
            result.SetExpirationDateTo(_repositoryTimePlusOneDay);

            result.Period.ValidFrom.Should().Be(_repositoryTime);
            result.Period.ValidTill.Should().HaveValue();
            result.Period.ValidTill.Should().Be(_repositoryTimePlusOneDay);
        }

        private DateTimeOffset GetCorrectParameterDate(DateTimeOffset date) =>
            date.Date.AddDays(1 - DateTime.Today.Day);
    }

    internal class DummySepsBaseAggregate : SepsBaseAggregate
    {
        public DummySepsBaseAggregate(Period period, IIdentityFactory<Guid> identityFactory)
            : base(period, identityFactory) { }
    }
}