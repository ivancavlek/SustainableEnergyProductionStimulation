using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class CogenerationTariffTests
    {
        private readonly CogenerationTariff _existingChp;
        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly YearlyAverageElectricEnergyProductionPrice _yaep;
        private readonly MonthlyAverageElectricEnergyProductionPrice _maep;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;

        private readonly decimal _lowerRate;
        private readonly decimal _higherRate;
        private readonly MonthlyPeriod _chpPeriod;
        private readonly decimal _lastQuarterGasPriceFor2006;
        private readonly decimal _maepPriceFor2006;

        public CogenerationTariffTests()
        {
            _lowerRate = 10M;
            _higherRate = 10M;
            _chpPeriod = new MonthlyPeriod(DateTime.Now.AddMonths(-4), DateTime.Now.AddMonths(-3));
            _identityFactory = new Mock<IIdentityFactory<Guid>>();
            _lastQuarterGasPriceFor2006 = 10M;
            _maepPriceFor2006 = 10M;

            _naturalGasSellingPrice = new NaturalGasSellingPrice(
                10M,
                nameof(NaturalGasSellingPrice),
                new MonthlyPeriod(DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(-2)),
                _identityFactory.Object);
            _maep = new MonthlyAverageElectricEnergyProductionPrice(
                10M,
                nameof(YearlyAverageElectricEnergyProductionPrice),
                new MonthlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                _identityFactory.Object);
            _yaep = new YearlyAverageElectricEnergyProductionPrice(
                Enumerable.Empty<MonthlyAverageElectricEnergyProductionPrice>(),
                10M,
                nameof(YearlyAverageElectricEnergyProductionPrice),
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
                new object[] { chpNaturalGasSellingPrice, _yaep, _lowerRate, _higherRate, _chpPeriod, _identityFactory.Object },
                null) as CogenerationTariff;
        }

        public void LastQuarterGasPriceFor2006MustBeGreaterThanZero()
        {
            Action action = () => _existingChp.CreateNewWith(
                0M, _maepPriceFor2006, null, _yaep, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.LastQuarterGasPriceFor2006Exception);
        }

        public void MaepPriceFor2006MustBeGreaterThanZero()
        {
            Action action = () => _existingChp.CreateNewWith(
                _lastQuarterGasPriceFor2006, 0M, _naturalGasSellingPrice, null, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.MaepPriceFor2006Exception);
        }

        public void NaturalGasSellingPriceMustBeSet()
        {
            Action action = () => _existingChp.CreateNewWith(
                _lastQuarterGasPriceFor2006, _maepPriceFor2006, null, _yaep, _identityFactory.Object);

            action
                .ShouldThrowExactly<ArgumentNullException>()
                .WithMessage(Infrastructure.Parameter.NaturalGasSellingPriceNotSetException);
        }

        public void YearlyAverageElectricEnergyProductionPriceMustBeSet()
        {
            Action action = () => _existingChp.CreateNewWith(
                _lastQuarterGasPriceFor2006, _maepPriceFor2006, _naturalGasSellingPrice, null, _identityFactory.Object);

            action
                .ShouldThrowExactly<ArgumentNullException>()
                .WithMessage(Infrastructure.Parameter.YaepNotSetException);
        }

        public void YaepPeriodIsUsedIfModifiedAfterNaturalGasPeriod()
        {
            var naturalGasSellingPrice = new NaturalGasSellingPrice(
                100M,
                nameof(NaturalGasSellingPrice),
                new MonthlyPeriod(DateTimeOffset.Now.AddYears(-4), DateTimeOffset.Now.AddYears(-3)),
                _identityFactory.Object);
            var newNaturalGasSellingPrice = new NaturalGasSellingPrice(
                100M,
                nameof(NaturalGasSellingPrice),
                new MonthlyPeriod(DateTimeOffset.Now.AddYears(-3), DateTimeOffset.Now.AddYears(-2)),
                _identityFactory.Object);
            var yaep = new YearlyAverageElectricEnergyProductionPrice(
                Enumerable.Empty<MonthlyAverageElectricEnergyProductionPrice>(),
                100M,
                nameof(YearlyAverageElectricEnergyProductionPrice),
                new YearlyPeriod(DateTimeOffset.Now.AddYears(-3), DateTimeOffset.Now.AddYears(-2)),
                _identityFactory.Object);
            var chp = Activator.CreateInstance(
                typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { naturalGasSellingPrice, _yaep, _lowerRate, _higherRate, naturalGasSellingPrice.Period, _identityFactory.Object },
                null) as CogenerationTariff;

            var newChp = chp.CreateNewWith(
                _lastQuarterGasPriceFor2006, _maepPriceFor2006, newNaturalGasSellingPrice, yaep, _identityFactory.Object);

            newChp.Period.Should().BeSameAs(yaep.Period);
        }

        public void NaturalGasPeriodIsUsedIfModifiedAfterYeap()
        {
            var naturalGasSellingPrice = new NaturalGasSellingPrice(
                100M,
                nameof(NaturalGasSellingPrice),
                new MonthlyPeriod(DateTimeOffset.Now.AddMonths(-4), DateTimeOffset.Now.AddMonths(-3)),
                _identityFactory.Object);
            var yaep = new YearlyAverageElectricEnergyProductionPrice(
                Enumerable.Empty<MonthlyAverageElectricEnergyProductionPrice>(),
                100M,
                nameof(YearlyAverageElectricEnergyProductionPrice),
                new YearlyPeriod(DateTimeOffset.Now.AddYears(-3), DateTimeOffset.Now.AddYears(-2)),
                _identityFactory.Object);
            var chp = Activator.CreateInstance(
                typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { naturalGasSellingPrice, yaep, _lowerRate, _higherRate, _chpPeriod, _identityFactory.Object },
                null) as CogenerationTariff;

            var newChp = _existingChp.CreateNewWith(
                 _lastQuarterGasPriceFor2006, _maepPriceFor2006, _naturalGasSellingPrice, _yaep, _identityFactory.Object);

            newChp.Period.Should().BeSameAs(_naturalGasSellingPrice.Period);
        }

        public void NewPeriodMustNotBeSmallerThenCurrentTariffValidFromAndCurrentMonthExclusive()
        {
            var naturalGasSellingPrice = new NaturalGasSellingPrice(
                100M,
                nameof(NaturalGasSellingPrice),
                new MonthlyPeriod(DateTimeOffset.Now.AddYears(-2), DateTimeOffset.Now.AddYears(-1)),
                _identityFactory.Object);
            var newYaep = new YearlyAverageElectricEnergyProductionPrice(
                Enumerable.Empty<MonthlyAverageElectricEnergyProductionPrice>(),
                100M,
                nameof(YearlyAverageElectricEnergyProductionPrice),
                new YearlyPeriod(DateTimeOffset.Now.AddYears(-4), DateTimeOffset.Now.AddYears(-3)),
                _identityFactory.Object);
            var chp = Activator.CreateInstance(
                typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { _naturalGasSellingPrice, _yaep, _lowerRate, _higherRate, _chpPeriod, _identityFactory.Object },
                null) as CogenerationTariff;

            Action action = () => chp.CreateNewWith(
                _lastQuarterGasPriceFor2006, _maepPriceFor2006, naturalGasSellingPrice, newYaep, _identityFactory.Object);

            action
                .ShouldThrowExactly<DomainException>()
                .WithMessage(Infrastructure.Parameter.ChpDateException);
        }

        public void RatesAreCorrectlyCalculated()
        {
            var result = _existingChp.CreateNewWith(
                _lastQuarterGasPriceFor2006, _maepPriceFor2006, _naturalGasSellingPrice, _yaep, _identityFactory.Object);

            result.LowerRate.Should().Be(10M);
            result.HigherRate.Should().Be(10M);
        }
    }
}