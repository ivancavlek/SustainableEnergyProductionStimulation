using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using FluentAssertions;
using Moq;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Entity
{
    public class SepsBaseEntityTests
    {
        private readonly Mock<ITimeZoneFactory> _timeZone;
        private readonly DateTimeOffset _repositoryTime;

        public SepsBaseEntityTests()
        {
            _timeZone = new Mock<ITimeZoneFactory>();
            _repositoryTime = DateTimeOffset.UtcNow;
        }

        public void CannotBeConstructedWithoutTimeZone()
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
            var repositoryTimeMinusOneMinute = _repositoryTime.AddMinutes(-1);
            var repositoryTimePlusOneMinute = _repositoryTime.AddMinutes(1);
            _timeZone.SetupSequence(m => m.GetCurrentRepositoryDateTime())
                .Returns(repositoryTimeMinusOneMinute)
                .Returns(_repositoryTime)
                .Returns(repositoryTimePlusOneMinute);

            var result = new DummySepsBaseEntity(_timeZone.Object);
            result.Archive();

            result.Period.ValidFrom.Should().Be(repositoryTimeMinusOneMinute);
            result.Period.ValidTill.Should().HaveValue();
            result.Period.ValidTill.Should().Be(repositoryTimePlusOneMinute);
        }

        public void CanOnlyBeArchivedOnce()
        {
            var repositoryTimeMinusOneMonth = _repositoryTime.AddMonths(-1);
            var repositoryTimePlusOneMinute = _repositoryTime.AddMinutes(1);
            _timeZone.SetupSequence(m => m.GetCurrentRepositoryDateTime())
                .Returns(repositoryTimeMinusOneMonth)
                .Returns(_repositoryTime)
                .Returns(_repositoryTime)
                .Returns(repositoryTimePlusOneMinute)
                .Returns(repositoryTimePlusOneMinute);

            var result = new DummySepsBaseEntity(_timeZone.Object);
            result.Archive();
            result.Archive();

            result.Period.ValidFrom.Should().Be(repositoryTimeMinusOneMonth);
            result.Period.ValidTill.Should().HaveValue();
            result.Period.ValidTill.Should().NotBe(repositoryTimePlusOneMinute);
            result.Period.ValidTill.Should().Be(_repositoryTime);
        }

        public void DeletesTheEntity()
        {
            _timeZone.Setup(m => m.GetCurrentRepositoryDateTime())
                .Returns(_repositoryTime);

            var result = new DummySepsBaseEntity(_timeZone.Object);
            result.Delete();

            result.Period.ValidFrom.Should().Be(_repositoryTime);
            result.Period.ValidTill.Should().HaveValue();
            result.Period.ValidTill.Should().Be(_repositoryTime);
        }

        public void DeleteCheckShowsCorrectDeletedStatus()
        {
            _timeZone.Setup(m => m.GetCurrentRepositoryDateTime())
                .Returns(_repositoryTime);

            var result = new DummySepsBaseEntity(_timeZone.Object);
            result.Delete();

            result.IsDeleted().Should().BeTrue();
        }

        public void DeleteCheckShowsCorrectExistingStatus()
        {
            _timeZone.Setup(m => m.GetCurrentRepositoryDateTime())
                .Returns(_repositoryTime);

            var result = new DummySepsBaseEntity(_timeZone.Object);

            result.IsDeleted().Should().BeFalse();
        }
    }

    internal class DummySepsBaseEntity : SepsBaseEntity
    {
        public DummySepsBaseEntity(ITimeZoneFactory timeZone) : base(timeZone)
        {
        }
    }
}