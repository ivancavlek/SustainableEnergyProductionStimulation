using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
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
            DateTimeOffset activeFrom = DateTimeOffset.Now.Date.AddMonths(-6);
            var activeTill = DateTimeOffset.Now.ToFirstDayOfTheMonth().AddMonths(-5);
            IEconometricIndexFactory<MonthlyAverageElectricEnergyProductionPrice> maepFactory =
                new EconometricIndexFactory<MonthlyAverageElectricEnergyProductionPrice>(activeFrom);
            var activeMaep = maepFactory.Create();

            var newMaep = activeMaep.CreateNew(
                amount, "Remark", activeTill.Month, activeTill.Year, Substitute.For<IIdentityFactory<Guid>>());

            activeMaep.Period.ActiveFrom.Should().Be(activeFrom.ToFirstDayOfTheMonth());
            activeMaep.Period.ActiveTill.Should().Be(activeTill.ToFirstDayOfTheMonth());
            newMaep.Period.ActiveFrom.Should().Be(activeTill.ToFirstDayOfTheMonth());
            newMaep.Period.ActiveTill.Should().BeNull();
            newMaep.Amount.Should().Be(Math.Round(amount, 4, MidpointRounding.AwayFromZero));
        }
    }
}