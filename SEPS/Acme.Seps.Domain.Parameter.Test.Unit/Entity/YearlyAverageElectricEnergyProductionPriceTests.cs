using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class YearlyAverageElectricEnergyProductionPriceTests
    {
        private readonly decimal _amount;
        private readonly string _remark;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;

        public YearlyAverageElectricEnergyProductionPriceTests()
        {
            _amount = 0.0909M;
            _remark = nameof(_remark);
            _identityFactory = new Mock<IIdentityFactory<Guid>>();
        }

        public void YaepIsProperlyConstructed()
        {
            var correctDate = DateTime.UtcNow.AddYears(-2);
            var period = new YearlyPeriod(correctDate.AddYears(-1), correctDate);

            var result = new YearlyAverageElectricEnergyProductionPrice(
                new List<MonthlyAverageElectricEnergyProductionPrice>
                {
                    new MonthlyAverageElectricEnergyProductionPrice(
                        1M,
                        "remark",
                        new MonthlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                        _identityFactory.Object)
                }, period, _identityFactory.Object);

            result.Amount.Should().Be(Math.Round(_amount, 4, MidpointRounding.AwayFromZero));
        }
    }
}