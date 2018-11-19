using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using NSubstitute;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class MonthlyAverageElectricEnergyProductionPriceTests
    {
        private readonly decimal _amount;
        private readonly string _remark;
        private readonly MonthlyAverageElectricEnergyProductionPrice _existingMaep;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public MonthlyAverageElectricEnergyProductionPriceTests()
        {
            _amount = 1.123456M;
            _remark = nameof(_remark);
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();

            _existingMaep = Activator.CreateInstance(
                typeof(MonthlyAverageElectricEnergyProductionPrice),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    10M,
                    nameof(MonthlyAverageElectricEnergyProductionPrice),
                    new Period(new MonthlyPeriodFactory(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2))),
                    _identityFactory },
                null) as MonthlyAverageElectricEnergyProductionPrice;
        }

        public void MonthlyAverageElectricEnergyProductionPriceIsProperlyConstructed()
        {
            var validTill = DateTime.Now.AddMonths(-5);

            var result = _existingMaep.CreateNew(
                _amount, _remark, validTill.Month, validTill.Year, _identityFactory);

            result.Amount.Should().Be(Math.Round(_amount, 4, MidpointRounding.AwayFromZero));
            _existingMaep.Period.ValidTill.Should().NotBeNull();
            _existingMaep.Period.ValidTill.Value
                .Should().Be(new DateTimeOffset(new DateTime(validTill.Year, validTill.Month, 1)));
            result.Period.ValidFrom.Should().Be(new DateTimeOffset(new DateTime(validTill.Year, validTill.Month, 1)));
            result.Period.ValidTill.Should().BeNull();
        }
    }
}