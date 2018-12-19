using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Test.Unit.Utility.Factory;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class YearlyAverageElectricEnergyProductionPriceTests
    {
        public void CreatesProperly()
        {
            const decimal amount = 1.123456M;
            DateTimeOffset since = DateTimeOffset.Now.Date.AddYears(-4);
            var until = DateTimeOffset.Now.ToFirstDayOfTheYear().AddYears(-3);
            IEconometricIndexFactory<YearlyAverageElectricEnergyProductionPrice> yaepFactory =
                new EconometricIndexFactory<YearlyAverageElectricEnergyProductionPrice>(since);
            var activeYaep = yaepFactory.Create();

            var newYaep = activeYaep.CreateNew(amount, "Remark", Substitute.For<IIdentityFactory<Guid>>());

            activeYaep.Active.Since.Should().Be(since.ToFirstDayOfTheYear());
            activeYaep.Active.Until.Should().Be(until);
            newYaep.Active.Since.Should().Be(until);
            newYaep.Active.Until.Should().BeNull();
            newYaep.Amount.Should().Be(Math.Round(amount, 4, MidpointRounding.AwayFromZero));
        }
    }
}