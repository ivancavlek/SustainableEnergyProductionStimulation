using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Test.Unit.Factory;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class ConsumerPriceIndexTests
    {
        public void CreatesProperly()
        {
            const decimal amount = 1.123456M;
            DateTimeOffset activeFrom = DateTimeOffset.Now.Date.AddYears(-4);
            var activeTill = DateTimeOffset.Now.ToFirstMonthOfTheYear().AddYears(-3);
            IEconometricIndexFactory<ConsumerPriceIndex> cpiFactory =
                new EconometricIndexFactory<ConsumerPriceIndex>(activeFrom);
            var activeCpi = cpiFactory.Create();

            var newCpi = activeCpi.CreateNew(amount, "Remark", Substitute.For<IIdentityFactory<Guid>>());

            activeCpi.Active.Since.Should().Be(activeFrom.ToFirstMonthOfTheYear());
            activeCpi.Active.Until.Should().Be(activeTill);
            newCpi.Active.Since.Should().Be(activeTill);
            newCpi.Active.Until.Should().BeNull();
            newCpi.Amount.Should().Be(Math.Round(amount, 4, MidpointRounding.AwayFromZero));
        }
    }
}