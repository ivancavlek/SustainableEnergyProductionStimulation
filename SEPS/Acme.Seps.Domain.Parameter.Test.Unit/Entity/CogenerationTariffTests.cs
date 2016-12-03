using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.DomainService;
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
        private readonly IEnumerable<NaturalGasSellingPrice> _yearsNaturalGasSellingPrices;
        private readonly Mock<ICogenerationParameterService> _cogenerationParameterService;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;
        private readonly MonthlyPeriod _chpPeriod;

        public CogenerationTariffTests()
        {
            _chpPeriod = new MonthlyPeriod(DateTime.Now.AddMonths(-4), DateTime.Now.AddMonths(-3));
            _identityFactory = new Mock<IIdentityFactory<Guid>>();
            _cogenerationParameterService = new Mock<ICogenerationParameterService>();
            _yearsNaturalGasSellingPrices = new List<NaturalGasSellingPrice> { _naturalGasSellingPrice };

            _naturalGasSellingPrice = Activator.CreateInstance(
                typeof(NaturalGasSellingPrice),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    10M,
                    nameof(NaturalGasSellingPrice),
                    new MonthlyPeriod(DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(-2)),
                    _identityFactory.Object },
                null) as NaturalGasSellingPrice;
            var chpNaturalGasSellingPrice = Activator.CreateInstance(
                typeof(NaturalGasSellingPrice),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    100M,
                    nameof(NaturalGasSellingPrice),
                    _chpPeriod,
                    _identityFactory.Object },
                null) as NaturalGasSellingPrice;
            _existingChp = Activator.CreateInstance(
                typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { chpNaturalGasSellingPrice, 10M, 10M, _chpPeriod, _identityFactory.Object },
                null) as CogenerationTariff;
        }

        public void YearsNaturalGasSellingPricesMustBeSet()
        {
            Action action = () => _existingChp.CreateNewWith(
                null, _cogenerationParameterService.Object, null, _identityFactory.Object);

            action
                .ShouldThrowExactly<ArgumentNullException>()
                .WithMessage(Infrastructure.Parameter.YearsNaturalGasSellingPricesException);
        }

        public void CogenerationParameterServiceMustBeSet()
        {
            Action action = () => _existingChp.CreateNewWith(
                _yearsNaturalGasSellingPrices, null, _naturalGasSellingPrice, _identityFactory.Object);

            action
                .ShouldThrowExactly<ArgumentNullException>()
                .WithMessage(Infrastructure.Parameter.CogenerationParameterServiceException);
        }

        public void NaturalGasSellingPriceMustBeSet()
        {
            Action action = () => _existingChp.CreateNewWith(
                _yearsNaturalGasSellingPrices, _cogenerationParameterService.Object, null, _identityFactory.Object);

            action
                .ShouldThrowExactly<ArgumentNullException>()
                .WithMessage(Infrastructure.Parameter.NaturalGasSellingPriceNotSetException);
        }

        public void ChpTariffIsCorrectlyConstructed()
        {
            var cogenerationParameter = 1M;

            _cogenerationParameterService
                .Setup(m => m.GetFrom(It.IsAny<IEnumerable<NaturalGasSellingPrice>>(), It.IsAny<NaturalGasSellingPrice>()))
                .Returns(cogenerationParameter);

            var result = _existingChp.CreateNewWith(
                _yearsNaturalGasSellingPrices,
                _cogenerationParameterService.Object,
                _naturalGasSellingPrice,
                _identityFactory.Object);

            _existingChp.Period.ValidTill.Should().Be(_naturalGasSellingPrice.Period.ValidFrom);
            result.LowerRate.Should().Be(_existingChp.LowerRate * cogenerationParameter);
            result.HigherRate.Should().Be(_existingChp.LowerRate * cogenerationParameter);
            result.NaturalGasSellingPrice.Should().Be(_naturalGasSellingPrice);
            result.Period.Should().Be(_naturalGasSellingPrice.Period);
        }
    }
}