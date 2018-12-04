using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class CogenerationTariffTests
    {
        private readonly CogenerationTariff _existingChp;
        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly IEnumerable<NaturalGasSellingPrice> _yearsNaturalGasSellingPrices;
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly IIdentityFactory<Guid> _identityFactory;
        private readonly Period _chpPeriod;

        public CogenerationTariffTests()
        {
            _chpPeriod = new Period(new MonthlyPeriodFactory(DateTime.Now.AddMonths(-4), DateTime.Now.AddMonths(-3)));
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
                    new Period(new MonthlyPeriodFactory(DateTime.Now.AddMonths(-4), DateTime.Now.AddMonths(-3))),
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
                new object[]
                {
                    chpNaturalGasSellingPrice,
                    100,
                    500,
                    10M,
                    10M,
                    Guid.NewGuid(),
                    new MonthlyPeriodFactory(DateTime.Now.AddMonths(-4), DateTime.Now.AddMonths(-3)),
                    _identityFactory },
                null) as CogenerationTariff;
        }

        public void CogenerationParameterServiceMustBeSet()
        {
            Action action = () => _existingChp.CreateNewWith(
                _yearsNaturalGasSellingPrices, null, _naturalGasSellingPrice, _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentNullException>()
                .WithMessage(SubsidyMessages.CogenerationParameterServiceException);
        }

        public void NaturalGasSellingPriceMustBeSet()
        {
            Action action = () => _existingChp.CreateNewWith(
                _yearsNaturalGasSellingPrices, _cogenerationParameterService, null, _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentNullException>()
                .WithMessage(SubsidyMessages.NaturalGasSellingPriceNotSetException);
        }

        public void ChpTariffIsCorrectlyConstructed()
        {
            const decimal cogenerationParameter = 1M;

            _cogenerationParameterService
                .GetFrom(Arg.Any<IEnumerable<NaturalGasSellingPrice>>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(cogenerationParameter);

            var result = _existingChp.CreateNewWith(
                _yearsNaturalGasSellingPrices,
                _cogenerationParameterService,
                _naturalGasSellingPrice,
                _identityFactory);

            _existingChp.Period.ValidTill.Should().Be(_naturalGasSellingPrice.Period.ValidFrom);
            result.LowerRate.Should().Be(_existingChp.LowerRate * cogenerationParameter);
            result.HigherRate.Should().Be(_existingChp.HigherRate * cogenerationParameter);
            result.NaturalGasSellingPrice.Should().Be(_naturalGasSellingPrice);
            result.Period.Should().Be(_naturalGasSellingPrice.Period);
        }
    }
}