using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Command.Entity;
using Acme.Seps.Domain.Subsidy.Test.Unit.Factory;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class MonthlyAverageElectricEnergyProductionPriceTests
    {
        public void CreatesProperly()
        {
            const decimal amount = 1.123456M;
            DateTimeOffset since = DateTimeOffset.Now.Date.AddMonths(-6);
            var until = DateTimeOffset.Now.ToFirstDayOfTheMonth().AddMonths(-5);
            IEconometricIndexFactory<MonthlyAverageElectricEnergyProductionPrice> maepFactory =
                new EconometricIndexFactory<MonthlyAverageElectricEnergyProductionPrice>(since);
            var activeMaep = maepFactory.Create();

            var newMaep = activeMaep.CreateNew(
                amount, "Remark", until.Month, until.Year, Substitute.For<IIdentityFactory<Guid>>());

            activeMaep.Active.Since.Should().Be(since.ToFirstDayOfTheMonth());
            activeMaep.Active.Until.Should().Be(until.ToFirstDayOfTheMonth());
            newMaep.Active.Since.Should().Be(until.ToFirstDayOfTheMonth());
            newMaep.Active.Until.Should().BeNull();
            newMaep.Amount.Should().Be(Math.Round(amount, 4, MidpointRounding.AwayFromZero));
        }
    }
}