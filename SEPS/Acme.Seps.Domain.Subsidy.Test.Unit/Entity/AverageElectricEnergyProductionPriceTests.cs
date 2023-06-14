using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Test.Unit.Utility.Factory;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity;

public class AverageElectricEnergyProductionPriceTests
{
    public void CreatesProperly()
    {
        const decimal amount = 1.123456M;
        DateTimeOffset since = DateTimeOffset.Now.Date.AddMonths(-6);
        var until = DateTimeOffset.Now.ToFirstDayOfTheMonth().AddMonths(-5);
        IEconometricIndexFactory<AverageElectricEnergyProductionPrice> aeeppFactory =
            new EconometricIndexFactory<AverageElectricEnergyProductionPrice>(since);
        var activeAeepp = aeeppFactory.Create();

        var newAeepp = activeAeepp.CreateNew(
            amount, "Remark", until.Month, until.Year, Substitute.For<IIdentityFactory<Guid>>());

        activeAeepp.Active.Since.Should().Be(since.ToFirstDayOfTheMonth());
        activeAeepp.Active.Until.Should().Be(until.ToFirstDayOfTheMonth());
        newAeepp.Active.Since.Should().Be(until.ToFirstDayOfTheMonth());
        newAeepp.Active.Until.Should().BeNull();
        newAeepp.Amount.Should().Be(Math.Round(amount, 4, MidpointRounding.AwayFromZero));
    }
}