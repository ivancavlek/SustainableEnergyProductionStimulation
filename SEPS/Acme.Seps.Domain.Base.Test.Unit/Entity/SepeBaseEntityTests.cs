using Acme.Domain.Base.DomainService;
using Acme.Seps.Domain.Base.Entity;
using FluentAssertions;
using Moq;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Entity
{
    public class SepsBaseEntityTests
    {
        private readonly Mock<ITimeZone> _timeZone;
        private readonly DateTimeOffset _repositoryTime;

        public SepsBaseEntityTests()
        {
            _timeZone = new Mock<ITimeZone>();
            _repositoryTime = DateTimeOffset.UtcNow;
        }

        public void ThrowsExceptionIfTimeZoneIsNotSet()
        {
            Action action = () => new DummySepsBaseEntity(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        public void SetsInitialPeriodUponCreation()
        {
            _timeZone.Setup(m => m.GetCurrentRepositoryDateTime()).Returns(_repositoryTime);

            var result = new DummySepsBaseEntity(_timeZone.Object);

            result.Period.ValidFrom.Should().Be(_repositoryTime);
            result.Period.ValidTill.Should().NotHaveValue();
        }

        public void ArchivesTheEntity()
        {
            var repositoryTimePlusOneMonth = _repositoryTime.AddMonths(1);
            _timeZone.SetupSequence(m => m.GetCurrentRepositoryDateTime())
                .Returns(_repositoryTime)
                .Returns(repositoryTimePlusOneMonth);

            var result = new DummySepsBaseEntity(_timeZone.Object);
            result.Archive();

            result.Period.ValidFrom.Should().Be(_repositoryTime);
            result.Period.ValidTill.Should().HaveValue();
            result.Period.ValidTill.Should().Be(repositoryTimePlusOneMonth);
        }

        public void CanOnlyBeArchivedOnce()
        {
            var repositoryTimePlusOneMonth = _repositoryTime.AddMonths(1);
            var repositoryTimePlusTwoMonth = _repositoryTime.AddMonths(2);
            _timeZone.SetupSequence(m => m.GetCurrentRepositoryDateTime())
                .Returns(_repositoryTime)
                .Returns(repositoryTimePlusOneMonth)
                .Returns(repositoryTimePlusTwoMonth);

            var result = new DummySepsBaseEntity(_timeZone.Object);
            result.Archive();
            result.Archive();

            result.Period.ValidFrom.Should().Be(_repositoryTime);
            result.Period.ValidTill.Should().HaveValue();
            result.Period.ValidTill.Should().NotBe(repositoryTimePlusTwoMonth);
            result.Period.ValidTill.Should().Be(repositoryTimePlusOneMonth);
        }
    }

    internal class DummySepsBaseEntity : SepsBaseEntity
    {
        public DummySepsBaseEntity(ITimeZone timeZone) : base(timeZone)
        {
        }
    }
}