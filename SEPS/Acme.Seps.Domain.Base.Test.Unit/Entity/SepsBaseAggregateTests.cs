using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Infrastructure;
using Acme.Seps.Domain.Base.Utility;
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
            _activeFrom = SepsVersion.InitialDate();
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

        public void OnlyDatesAfterInitialDateAreValid()
        {
            Action action = () => new DummySepsBaseAggregate(_activeFrom.AddYears(-1), _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentOutOfRangeException>()
                .WithMessage(SepsBaseMessage.DateMustBeGreaterThanInitialDate);
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

        public void ActiveFromIsCorrected()
        {
            var result = new DummySepsBaseAggregate(_activeFrom, _identityFactory);
            var period = result.Period;
            var oldActiveTill = period.ActiveTill;

            var newDate = _activeFrom.AddYears(1);

            result.CorrectActiveFrom(newDate);

            result.Period.Should().NotBe(period);
            result.Period.ActiveFrom.Should().Be(newDate);
            result.Period.ActiveTill.Should().Be(oldActiveTill);
        }

        public void ActiveTillIsCorrected()
        {
            var result = new DummySepsBaseAggregate(_activeFrom, _identityFactory);
            var period = result.Period;
            var oldActiveFrom = period.ActiveFrom;

            var newDate = _activeFrom.AddYears(1);

            result.CorrectActiveTill(newDate);

            result.Period.Should().NotBe(period);
            result.Period.ActiveTill.Should().Be(newDate);
            result.Period.ActiveFrom.Should().Be(oldActiveFrom);
        }

        public void EntityWithActiveTillDateIsInActive()
        {
            var dateAfterInitialDate = SepsVersion.InitialDate().AddYears(1);

            var result = new DummySepsBaseAggregate(dateAfterInitialDate, _identityFactory);
            result.Archive(new DateTime(2019, 01, 01));

            result.IsActive().Should().BeFalse();
        }

        public void EntityIsActive()
        {
            var dateAfterInitialDate = SepsVersion.InitialDate().AddYears(1);

            var result = new DummySepsBaseAggregate(dateAfterInitialDate, _identityFactory);

            result.IsActive().Should().BeTrue();
        }

        private class DummySepsBaseAggregate : SepsAggregateRoot
        {
            public DummySepsBaseAggregate(DateTimeOffset activeFrom, IIdentityFactory<Guid> identityFactory)
                : base(activeFrom, identityFactory)
            {
            }

            internal new void Archive(DateTimeOffset activeTill)
            {
                base.Archive(activeTill);
            }

            internal new void CorrectActiveFrom(DateTimeOffset activeFrom)
            {
                base.CorrectActiveFrom(activeFrom);
            }

            internal new void CorrectActiveTill(DateTimeOffset activeTill)
            {
                base.CorrectActiveTill(activeTill);
            }
        }
    }
}