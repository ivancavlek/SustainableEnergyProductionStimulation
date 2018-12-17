using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Test.Unit.Factory;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class NaturalGasSellingPriceTests
    {
        public void CreatesProperly()
        {
            const decimal amount = 1.123456M;
            DateTimeOffset activeFrom = DateTimeOffset.Now.Date.AddMonths(-6);
            var activeTill = DateTimeOffset.Now.ToFirstDayOfTheMonth().AddMonths(-5);
            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(activeFrom);
            var activeNgsp = ngspFactory.Create();

            var newMaep = activeNgsp.CreateNew(
                amount, "Remark", activeTill.Month, activeTill.Year, Substitute.For<IIdentityFactory<Guid>>());

            activeNgsp.Active.Since.Should().Be(activeFrom.ToFirstDayOfTheMonth());
            activeNgsp.Active.Until.Should().Be(activeTill.ToFirstDayOfTheMonth());
            newMaep.Active.Since.Should().Be(activeTill.ToFirstDayOfTheMonth());
            newMaep.Active.Until.Should().BeNull();
            newMaep.Amount.Should().Be(Math.Round(amount, 2, MidpointRounding.AwayFromZero));
        }
    }
}