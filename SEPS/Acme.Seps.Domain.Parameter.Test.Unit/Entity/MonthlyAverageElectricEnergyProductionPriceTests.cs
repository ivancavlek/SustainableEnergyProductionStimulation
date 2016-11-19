using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using Moq;
using System;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class MonthlyAverageElectricEnergyProductionPriceTests
    {
        private readonly decimal _amount;
        private readonly string _remark;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;

        public MonthlyAverageElectricEnergyProductionPriceTests()
        {
            _amount = 1.123456M;
            _remark = nameof(_remark);
            _identityFactory = new Mock<IIdentityFactory<Guid>>();
        }

        public void AmountIsProperlyRounded()
        {
            var correctDate = DateTime.UtcNow.AddMonths(-2);
            var period = new MonthlyPeriod(correctDate.AddMonths(-2), correctDate);

            var result = new MonthlyAverageElectricEnergyProductionPrice(
                _amount, _remark, period, _identityFactory.Object);

            result.Amount.Should().Be(Math.Round(_amount, 4, MidpointRounding.AwayFromZero));
        }
    }
}