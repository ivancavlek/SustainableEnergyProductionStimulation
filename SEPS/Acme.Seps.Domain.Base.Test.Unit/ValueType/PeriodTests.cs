using Acme.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Infrastructure;
using Acme.Seps.Domain.Base.ValueType;
using FluentAssertions;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.ValueType
{
    public class PeriodTests
    {
        private readonly DateTimeOffset _activeFrom;
        private readonly DateTimeOffset _activeTill;
        private readonly Period _period;

        public PeriodTests()
        {
            _activeFrom = new DateTime(2000, 1, 1);
            _activeTill = _activeFrom.AddYears(1);
            _period = new Period(_activeFrom);
        }

        public void CreatesProperlyWithActiveFrom()
        {
            _period.ActiveFrom.Should().Be(_activeFrom);
        }

        public void SetActiveTillArchivesThePeriod()
        {
            var newPeriod = _period.SetActiveTill(_activeTill);

            _period.Should().NotBe(newPeriod);
            _period.ActiveFrom.Should().Be(_activeFrom);
            _period.ActiveTill.Should().BeNull();
            newPeriod.ActiveFrom.Should().Be(_activeFrom);
            newPeriod.ActiveTill.Should().Be(_activeTill);
        }

        public void ActiveFromMustNotBeLowerThanActiveTill()
        {
            Action action = () => _period.SetActiveTill(_activeFrom.AddYears(-1));

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SepsBaseMessage.ValidTillGreaterThanValidFromException);
        }
    }
}