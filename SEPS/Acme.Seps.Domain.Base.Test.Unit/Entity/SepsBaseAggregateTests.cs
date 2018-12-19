using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Text;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Entity
{
    public class SepsBaseAggregateTests
    {
        private readonly IIdentityFactory<Guid> _identityFactory;
        private readonly DateTimeOffset _activeSince;

        public SepsBaseAggregateTests()
        {
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _identityFactory.CreateIdentity().Returns(Guid.NewGuid());
            _activeSince = SepsVersion.InitialDate();
        }

        public void CreatesProperly()
        {
            var result = new DummySepsBaseAggregate(_activeSince, _identityFactory);

            result.Active.Since.Should().Be(_activeSince);
        }

        public void SetInactivesEntity()
        {
            var activeUntil = _activeSince.AddYears(1);

            var result = new DummySepsBaseAggregate(_activeSince, _identityFactory);
            var oldPeriod = result.Active;
            result.SetInactive(activeUntil);

            result.Active.Should().NotBe(oldPeriod);
            result.Active.Since.Should().Be(_activeSince);
            result.Active.Until.Should().Be(activeUntil);
        }

        public void OnlyDatesAfterInitialDateAreValid()
        {
            Action action = () => new DummySepsBaseAggregate(_activeSince.AddYears(-1), _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentOutOfRangeException>()
                .WithMessage(SepsMessage.DateMustBeGreaterThanInitialDate);
        }

        public void OnlyActiveEntityCanBeSetInactived()
        {
            var activeUntil = _activeSince.AddYears(1);

            var result = new DummySepsBaseAggregate(_activeSince, _identityFactory);
            result.SetInactive(activeUntil);

            Action action = () => result.SetInactive(activeUntil);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SepsMessage.ArchivingArchivedEntityException);
        }

        public void ActiveSinceIsCorrected()
        {
            var result = new DummySepsBaseAggregate(_activeSince, _identityFactory);
            var activePeriod = result.Active;
            var oldActiveUntil = activePeriod.Until;

            var newDate = _activeSince.AddYears(1);

            result.CorrectActiveSince(newDate);

            result.Active.Should().NotBe(activePeriod);
            result.Active.Since.Should().Be(newDate);
            result.Active.Until.Should().Be(oldActiveUntil);
        }

        public void ActiveUntilIsCorrected()
        {
            var result = new DummySepsBaseAggregate(_activeSince, _identityFactory);
            var activePeriod = result.Active;
            var oldActiveSince = activePeriod.Since;

            var newDate = _activeSince.AddYears(1);

            result.CorrectActiveUntil(newDate);

            result.Active.Should().NotBe(activePeriod);
            result.Active.Until.Should().Be(newDate);
            result.Active.Since.Should().Be(oldActiveSince);
        }

        public void EntityWithActiveUntilDateIsInactive()
        {
            var dateAfterInitialDate = SepsVersion.InitialDate().AddYears(1);

            var result = new DummySepsBaseAggregate(dateAfterInitialDate, _identityFactory);
            result.SetInactive(new DateTime(2019, 01, 01));

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
            public DummySepsBaseAggregate(DateTimeOffset since, IIdentityFactory<Guid> identityFactory)
                : base(since, identityFactory)
            {
            }

            internal new void SetInactive(DateTimeOffset inactiveFrom)
            {
                base.SetInactive(inactiveFrom);
            }

            internal new void CorrectActiveSince(DateTimeOffset activeSince)
            {
                base.CorrectActiveSince(activeSince);
            }

            internal new void CorrectActiveUntil(DateTimeOffset activeUntil)
            {
                base.CorrectActiveUntil(activeUntil);
            }
        }
    }
}