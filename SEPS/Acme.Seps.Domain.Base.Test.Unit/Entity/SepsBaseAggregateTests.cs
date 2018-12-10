using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Infrastructure;
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

        public void OnlyActiveEntityCanBeArchived()
        {
            var activeTill = _activeFrom.AddYears(1);

            var result = new DummySepsBaseAggregate(_activeFrom, _identityFactory);
            result.Archive(activeTill);

            Action action = () => result.Archive(activeTill);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SepsBaseMessage.ArchivingArchivedEntityException);
        }

        public void EntityWithActiveTillDateIsInActive()
        {
            var dateAfter20070101 = new DateTime(2008, 01, 01);

            var result = new DummySepsBaseAggregate(dateAfter20070101, _identityFactory);
            result.Archive(new DateTime(2019, 01, 01));

            result.IsActive().Should().BeFalse();
        }

        public void EntityWithActiveFromDateBefore20170101IsInactive()
        {
            var dateBefore20070101 = new DateTime(2006, 01, 01);

            var result = new DummySepsBaseAggregate(dateBefore20070101, _identityFactory);

            result.IsActive().Should().BeFalse();
        }

        public void EntityIsActive()
        {
            var dateAfter20070101 = new DateTime(2008, 01, 01);

            var result = new DummySepsBaseAggregate(dateAfter20070101, _identityFactory);

            result.IsActive().Should().BeTrue();
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