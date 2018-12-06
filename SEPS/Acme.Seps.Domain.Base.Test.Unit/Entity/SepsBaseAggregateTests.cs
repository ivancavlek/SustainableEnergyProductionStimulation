using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Entity
{
    public class SepsBaseAggregateTests
    {
        private readonly IIdentityFactory<Guid> _identityFactory;
        private readonly DateTimeOffset _activeFrom;

        public SepsBaseAggregateTests()
        {
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _identityFactory.CreateIdentity().Returns(Guid.NewGuid());
            _activeFrom = new DateTime(2000, 1, 1);
        }

        public void CreatesProperly()
        {
            var result = new DummySepsBaseAggregate(_activeFrom, _identityFactory);

            result.Period.ActiveFrom.Should().Be(_activeFrom);
        }

        public void ArchivesEntity()
        {
            var activeTill = _activeFrom.AddYears(1);

            var result = new DummySepsBaseAggregate(_activeFrom, _identityFactory);
            var oldPeriod = result.Period;
            result.Archive(activeTill);

            result.Period.Should().NotBe(oldPeriod);
            result.Period.ActiveFrom.Should().Be(_activeFrom);
            result.Period.ActiveTill.Should().Be(activeTill);
        }

        private class DummySepsBaseAggregate : SepsAggregateRoot
        {
            public DummySepsBaseAggregate(DateTimeOffset activeFrom, IIdentityFactory<Guid> identityFactory)
                : base(activeFrom, identityFactory)
            {
            }
        }
    }
}