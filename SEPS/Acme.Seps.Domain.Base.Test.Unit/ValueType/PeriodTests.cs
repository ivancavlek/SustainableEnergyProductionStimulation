using Acme.Domain.Base.Entity;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Text;
using FluentAssertions;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.ValueType
{
    public class PeriodTests
    {
        private readonly DateTimeOffset _activeSince;
        private readonly DateTimeOffset _activeUntil;
        private readonly ActivePeriod _activePeriod;

        public PeriodTests()
        {
            _activeSince = new DateTime(2000, 1, 1);
            _activeUntil = _activeSince.AddYears(1);
            _activePeriod = new ActivePeriod(_activeSince);
        }

        public void CreatesProperlyWithActiveSince()
        {
            _activePeriod.Since.Should().Be(_activeSince);
        }

        public void SetActiveUntilInactivatesThePeriod()
        {
            var newPeriod = _activePeriod.SetActiveUntil(_activeUntil);

            _activePeriod.Should().NotBe(newPeriod);
            _activePeriod.Since.Should().Be(_activeSince);
            _activePeriod.Until.Should().BeNull();
            newPeriod.Since.Should().Be(_activeSince);
            newPeriod.Until.Should().Be(_activeUntil);
        }

        public void ActiveSinceMustNotBeLowerThanActiveUntil()
        {
            var until = _activeSince.AddYears(-1);

            Action action = () => _activePeriod.SetActiveUntil(until);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SepsMessage.ValueHigherThanTheOther(
                    until.Date.ToShortDateString(), _activePeriod.Since.Date.ToShortDateString()));
        }
    }
}