using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.DomainService;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using NSubstitute;
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
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly IIdentityFactory<Guid> _identityFactory;
        private readonly MonthlyPeriod _chpPeriod;

        public CogenerationTariffTests()
        {
            _chpPeriod = new MonthlyPeriod(DateTime.Now.AddMonths(-4), DateTime.Now.AddMonths(-3));
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _cogenerationParameterService = Substitute.For<ICogenerationParameterService>();
            _yearsNaturalGasSellingPrices = new List<NaturalGasSellingPrice> { _naturalGasSellingPrice };

            _naturalGasSellingPrice = Activator.CreateInstance(
                typeof(NaturalGasSellingPrice),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    10M,
                    nameof(NaturalGasSellingPrice),
                    new MonthlyPeriod(DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(-2)),
                    _identityFactory },
                null) as NaturalGasSellingPrice;
            var chpNaturalGasSellingPrice = Activator.CreateInstance(
                typeof(NaturalGasSellingPrice),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    100M,
                    nameof(NaturalGasSellingPrice),
                    _chpPeriod,
                    _identityFactory },
                null) as NaturalGasSellingPrice;
            _existingChp = Activator.CreateInstance(
                typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { chpNaturalGasSellingPrice, 10M, 10M, _chpPeriod, _identityFactory },
                null) as CogenerationTariff;
        }

        public void YearsNaturalGasSellingPricesMustBeSet()
        {
            Action action = () => _existingChp.CreateNewWith(
                null, _cogenerationParameterService, null, _identityFactory);

            action
                .Should()
                .Throw<Exception>()
                .WithMessage(Infrastructure.Parameter.YearsNaturalGasSellingPricesException);
        }

        public void CogenerationParameterServiceMustBeSet()
        {
            Action action = () => _existingChp.CreateNewWith(
                _yearsNaturalGasSellingPrices, null, _naturalGasSellingPrice, _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentNullException>()
                .WithMessage(Infrastructure.Parameter.CogenerationParameterServiceException);
        }

        public void NaturalGasSellingPriceMustBeSet()
        {
            Action action = () => _existingChp.CreateNewWith(
                _yearsNaturalGasSellingPrices, _cogenerationParameterService, null, _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentNullException>()
                .WithMessage(Infrastructure.Parameter.NaturalGasSellingPriceNotSetException);
        }

        public void ChpTariffIsCorrectlyConstructed()
        {
            var cogenerationParameter = 1M;

            _cogenerationParameterService
                .GetFrom(Arg.Any<IEnumerable<NaturalGasSellingPrice>>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(cogenerationParameter);

            var result = _existingChp.CreateNewWith(
                _yearsNaturalGasSellingPrices,
                _cogenerationParameterService,
                _naturalGasSellingPrice,
                _identityFactory);

            _existingChp.MonthlyPeriod.ValidTill.Should().Be(_naturalGasSellingPrice.MonthlyPeriod.ValidFrom);
            result.LowerRate.Should().Be(_existingChp.LowerRate * cogenerationParameter);
            result.HigherRate.Should().Be(_existingChp.LowerRate * cogenerationParameter);
            result.NaturalGasSellingPrice.Should().Be(_naturalGasSellingPrice);
            result.MonthlyPeriod.Should().Be(_naturalGasSellingPrice.MonthlyPeriod);
        }
    }
}