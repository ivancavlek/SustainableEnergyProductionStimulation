using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using Moq;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class MonthlyAverageElectricEnergyProductionPriceTests
    {
        private readonly decimal _amount;
        private readonly string _remark;
        private readonly MonthlyAverageElectricEnergyProductionPrice _existingMaep;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;

        public MonthlyAverageElectricEnergyProductionPriceTests()
        {
            _amount = 1.123456M;
            _remark = nameof(_remark);
            _identityFactory = new Mock<IIdentityFactory<Guid>>();

            _existingMaep = Activator.CreateInstance(
                typeof(MonthlyAverageElectricEnergyProductionPrice),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    10M,
                    nameof(MonthlyAverageElectricEnergyProductionPrice),
                    new MonthlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                    _identityFactory.Object },
                null) as MonthlyAverageElectricEnergyProductionPrice;
        }

        public void AmountIsProperlyRounded()
        {
            var validTill = DateTime.Now.AddMonths(-5);

            var result = _existingMaep.CreateNew(_amount, _remark, validTill, _identityFactory.Object);

            result.Amount.Should().Be(Math.Round(_amount, 4, MidpointRounding.AwayFromZero));
        }
    }
}