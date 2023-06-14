using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Text;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Repository;

public class PreviousActiveSpecificationTests
{
    private readonly DummySepsBaseAggregate _dummy;
    private readonly IIdentityFactory<Guid> _identityFactory;

    public PreviousActiveSpecificationTests()
    {
        _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        _identityFactory.CreateIdentity().Returns(Guid.NewGuid());
        var activeSince = SepsVersion.InitialDate();

        _dummy = new DummySepsBaseAggregate(activeSince, _identityFactory);
    }

    public void AggregateRootMustBeSet()
    {
        Action action = () => new PreviousActiveSpecification<DummySepsBaseAggregate>(null);

        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    public void AggregateRootMustBeActive()
    {
        _dummy.SetInactive(SepsVersion.InitialDate().AddYears(1));

        Action action = () => new PreviousActiveSpecification<DummySepsBaseAggregate>(_dummy);

        action
            .Should()
            .Throw<Exception>()
            .WithMessage(SepsMessage.InactiveException("DummySepsBaseAggregate"));
    }

    public void FiltersActiveEntitites()
    {
        var dummyDate = SepsVersion.InitialDate().AddYears(1);

        var previousActiveDummy = new DummySepsBaseAggregate(SepsVersion.InitialDate(), _identityFactory);
        previousActiveDummy.SetInactive(dummyDate);

        var activeDummy = new DummySepsBaseAggregate(dummyDate, _identityFactory);

        new PreviousActiveSpecification<DummySepsBaseAggregate>(activeDummy)
            .IsSatisfiedBy(previousActiveDummy).Should().BeTrue();
    }

    private class DummySepsBaseAggregate : SepsAggregateRoot
    {
        public DummySepsBaseAggregate(DateTimeOffset since, IIdentityFactory<Guid> identityFactory)
            : base(since, identityFactory) { }

        internal new void SetInactive(DateTimeOffset inactiveFrom) => base.SetInactive(inactiveFrom);

        internal new void CorrectActiveSince(DateTimeOffset activeSince) => base.CorrectActiveSince(activeSince);

        internal new void CorrectActiveUntil(DateTimeOffset activeUntil) => base.CorrectActiveUntil(activeUntil);
    }
}