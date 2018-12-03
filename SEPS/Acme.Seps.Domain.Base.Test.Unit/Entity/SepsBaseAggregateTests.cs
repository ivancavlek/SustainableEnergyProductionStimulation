using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Factory;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Entity
{
    public class SepsBaseAggregateTests
    {
        private readonly IIdentityFactory<Guid> _identityFactory;

        public SepsBaseAggregateTests()
        {
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _identityFactory.CreateIdentity().Returns(Guid.NewGuid());
        }

        public void PeriodFactoryMustBeSet()
        {
            Action action = () => new DummySepsBaseAggregate(null, _identityFactory);

            action.Should().Throw<Exception>();
        }

        public void ExecutesProperly()
        {
            var result = new DummySepsBaseAggregate(new MonthlyPeriodFactory(DateTime.Now), _identityFactory);

            result.Period.Should().NotBeNull();
        }
    }

    internal class DummySepsBaseAggregate : SepsAggregateRoot
    {
        public DummySepsBaseAggregate(IPeriodFactory periodFactory, IIdentityFactory<Guid> identityFactory)
            : base(periodFactory, identityFactory)
        {
        }
    }
}