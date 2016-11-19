using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using Moq;
using System;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class YearlyAverageElectricEnergyProductionPriceTests
    {
        private readonly decimal _amount;
        private readonly string _remark;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;

        public YearlyAverageElectricEnergyProductionPriceTests()
        {
            _amount = 1.123456M;
            _remark = nameof(_remark);
            _identityFactory = new Mock<IIdentityFactory<Guid>>();
        }

        public void AmountIsProperlyRounded()
        {
            var correctDate = DateTime.UtcNow.AddYears(-2);
            var period = new YearlyPeriod(correctDate.AddYears(-1), correctDate);

            var result = new YearlyAverageElectricEnergyProductionPrice(
                Enumerable.Empty<MonthlyAverageElectricEnergyProductionPrice>(), _amount, _remark, period, _identityFactory.Object);

            result.Amount.Should().Be(Math.Round(_amount, 4, MidpointRounding.AwayFromZero));
        }
    }
}