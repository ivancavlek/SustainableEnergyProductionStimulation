using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.UseCases.Subsidy.Command.Entity;
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
            DateTimeOffset since = DateTimeOffset.Now.Date.AddYears(-4);
            var until = DateTimeOffset.Now.ToFirstMonthOfTheYear().AddYears(-3);
            IEconometricIndexFactory<ConsumerPriceIndex> cpiFactory =
                new EconometricIndexFactory<ConsumerPriceIndex>(since);
            var activeCpi = cpiFactory.Create();

            var newCpi = activeCpi.CreateNew(amount, "Remark", Substitute.For<IIdentityFactory<Guid>>());

            activeCpi.Active.Since.Should().Be(since.ToFirstMonthOfTheYear());
            activeCpi.Active.Until.Should().Be(until);
            newCpi.Active.Since.Should().Be(until);
            newCpi.Active.Until.Should().BeNull();
            newCpi.Amount.Should().Be(Math.Round(amount, 4, MidpointRounding.AwayFromZero));
        }
    }
}