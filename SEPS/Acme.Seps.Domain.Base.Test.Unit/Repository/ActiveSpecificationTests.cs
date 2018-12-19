using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Utility;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Repository
{
    public class ActiveSpecificationTests
    {
        private readonly DummySepsBaseAggregate _dummy;
        private readonly ActiveSpecification<DummySepsBaseAggregate> _activeSpecification;

        public ActiveSpecificationTests()
        {
            var identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            identityFactory.CreateIdentity().Returns(Guid.NewGuid());
            var activeSince = SepsVersion.InitialDate();

            _dummy = new DummySepsBaseAggregate(activeSince, identityFactory);

            _activeSpecification = new ActiveSpecification<DummySepsBaseAggregate>();
        }

        public void FiltersActiveEntitites()
        {
            _activeSpecification.IsSatisfiedBy(_dummy).Should().BeTrue();
        }

        public void IgnoresInactiveEntitites()
        {
            _dummy.SetInactive(SepsVersion.InitialDate().AddYears(1));

            _activeSpecification.IsSatisfiedBy(_dummy).Should().BeFalse();
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