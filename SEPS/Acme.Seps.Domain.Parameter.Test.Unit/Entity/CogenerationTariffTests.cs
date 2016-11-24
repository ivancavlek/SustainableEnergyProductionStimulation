using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class CogenerationTariffTests
    {
        private readonly CogenerationTariff _existingChp;
        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly YearlyAverageElectricEnergyProductionPrice _yaep;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;
        private readonly MonthlyPeriod _chpPeriod;

        public CogenerationTariffTests()
        {
            _chpPeriod = new MonthlyPeriod(DateTime.Now.AddMonths(-4), DateTime.Now.AddMonths(-3));
            _identityFactory = new Mock<IIdentityFactory<Guid>>();

            _naturalGasSellingPrice = new NaturalGasSellingPrice(
                10M,
                nameof(NaturalGasSellingPrice),
                new MonthlyPeriod(DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(-2)),
                _identityFactory.Object);
            var maep = new MonthlyAverageElectricEnergyProductionPrice(
                10M,
                nameof(YearlyAverageElectricEnergyProductionPrice),
                new MonthlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                _identityFactory.Object);
            _yaep = new YearlyAverageElectricEnergyProductionPrice(
                new List<MonthlyAverageElectricEnergyProductionPrice>
                {
                    new MonthlyAverageElectricEnergyProductionPrice(
                        1M,
                        "remark",
                        new MonthlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                        _identityFactory.Object)
                },
                new YearlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                _identityFactory.Object);

            var chpNaturalGasSellingPrice = new NaturalGasSellingPrice(
                100M,
                nameof(NaturalGasSellingPrice),
                _chpPeriod,
                _identityFactory.Object);

            _existingChp = Activator.CreateInstance(
                typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { chpNaturalGasSellingPrice, _yaep, 10M, 10M, _chpPeriod, _identityFactory.Object },
                null) as CogenerationTariff;
        }

        public void NaturalGasSellingPriceMustBeSet()
        {
            Action action = () => _existingChp.CreateNewWith(
                1M, _chpPeriod, null, _yaep, _identityFactory.Object);

            action
                .ShouldThrowExactly<ArgumentNullException>()
                .WithMessage(Infrastructure.Parameter.NaturalGasSellingPriceNotSetException);
        }

        public void YearlyAverageElectricEnergyProductionPriceMustBeSet()
        {
            Action action = () => _existingChp.CreateNewWith(
                1M, _chpPeriod, _naturalGasSellingPrice, null, _identityFactory.Object);

            action
                .ShouldThrowExactly<ArgumentNullException>()
                .WithMessage(Infrastructure.Parameter.YaepNotSetException);
        }

        public void NewMonthlyPeriodMustNotBeSmallerThenCurrentTariffValidFrom()
        {
            var falsePeriod = new MonthlyPeriod(DateTime.Now.AddYears(-3));

            Action action = () => _existingChp.CreateNewWith(
                1M, falsePeriod, _naturalGasSellingPrice, _yaep, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.ChpDateException);
        }

        public void NewYearlyPeriodMustNotBeSmallerThenCurrentTariffValidFrom()
        {
            var falsePeriod = new YearlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2));

            Action action = () => _existingChp.CreateNewWith(
                1M, falsePeriod, _naturalGasSellingPrice, _yaep, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.ChpDateException);
        }

        public void ChpTariffIsCorrectlyConstructed()
        {
            var result = _existingChp.CreateNewWith(
                1M, _chpPeriod, _naturalGasSellingPrice, _yaep, _identityFactory.Object);

            _existingChp.Period.ValidTill.Should().Be(_chpPeriod.ValidFrom);
            result.LowerRate.Should().Be(10M);
            result.HigherRate.Should().Be(10M);
            result.NaturalGasSellingPrice.Should().Be(_naturalGasSellingPrice);
            result.YearlyAverageElectricEnergyProductionPrice.Should().Be(_yaep);
            result.Period.Should().Be(_chpPeriod);
        }
    }
}