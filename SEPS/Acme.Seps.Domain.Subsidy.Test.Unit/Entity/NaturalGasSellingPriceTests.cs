using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.UseCases.Subsidy.Command.Entity;
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
            DateTimeOffset since = DateTimeOffset.Now.Date.AddMonths(-6);
            var until = DateTimeOffset.Now.ToFirstDayOfTheMonth().AddMonths(-5);
            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(since);
            var activeNgsp = ngspFactory.Create();

            var newMaep = activeNgsp.CreateNew(
                amount, "Remark", until.Month, until.Year, Substitute.For<IIdentityFactory<Guid>>());

            activeNgsp.Active.Since.Should().Be(since.ToFirstDayOfTheMonth());
            activeNgsp.Active.Until.Should().Be(until.ToFirstDayOfTheMonth());
            newMaep.Active.Since.Should().Be(until.ToFirstDayOfTheMonth());
            newMaep.Active.Until.Should().BeNull();
            newMaep.Amount.Should().Be(Math.Round(amount, 2, MidpointRounding.AwayFromZero));
        }
    }
}