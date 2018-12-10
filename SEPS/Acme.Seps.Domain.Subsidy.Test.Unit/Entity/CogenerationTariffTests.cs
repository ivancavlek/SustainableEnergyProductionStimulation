using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Infrastructure;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using Acme.Seps.Domain.Subsidy.Test.Unit.Factory;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class CogenerationTariffTests
    {
        private readonly CogenerationTariff _activeCgn;
        private readonly NaturalGasSellingPrice _newNaturalGasSellingPrice;
        private readonly IEnumerable<NaturalGasSellingPrice> _yearsNaturalGasSellingPrices;
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CogenerationTariffTests()
        {
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _cogenerationParameterService = Substitute.For<ICogenerationParameterService>();
            _yearsNaturalGasSellingPrices = new List<NaturalGasSellingPrice> { _newNaturalGasSellingPrice };

            DateTimeOffset cgnActiveFrom = DateTimeOffset.Now.ToFirstDayOfTheMonth().AddMonths(-9);
            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(cgnActiveFrom);
            ITariffFactory<CogenerationTariff> cogenerationFactory =
                new TariffFactory<CogenerationTariff>(ngspFactory.Create());
            _activeCgn = cogenerationFactory.Create();

            DateTimeOffset ngspActiveFrom = DateTimeOffset.Now.ToFirstDayOfTheMonth().AddMonths(-4);
            ngspFactory = new EconometricIndexFactory<NaturalGasSellingPrice>(ngspActiveFrom);
            _newNaturalGasSellingPrice = ngspFactory.Create();
        }

        public void CogenerationParameterServiceMustBeSet()
        {
            Action action = () => _activeCgn.CreateNewWith(
                _yearsNaturalGasSellingPrices, null, _newNaturalGasSellingPrice, _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentNullException>()
                .WithMessage(SubsidyMessages.CogenerationParameterServiceException);
        }

        public void NaturalGasSellingPriceMustBeSet()
        {
            Action action = () => _activeCgn.CreateNewWith(
                _yearsNaturalGasSellingPrices, _cogenerationParameterService, null, _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentNullException>()
                .WithMessage(SubsidyMessages.NaturalGasSellingPriceNotSetException);
        }

        public void NaturalGasSellingPriceMustBeActive()
        {
            _newNaturalGasSellingPrice.Archive(DateTime.Now);

            Action action = () => _activeCgn.CreateNewWith(
                _yearsNaturalGasSellingPrices,
                _cogenerationParameterService,
                _newNaturalGasSellingPrice,
                _identityFactory);

            action
                .Should()
                .Throw<Exception>()
                .WithMessage(SepsBaseMessage.InactiveException);
        }

        public void CreatesProperly()
        {
            const decimal cogenerationParameter = 1M;

            _cogenerationParameterService
                .GetFrom(Arg.Any<IEnumerable<NaturalGasSellingPrice>>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(cogenerationParameter);

            var newChp = _activeCgn.CreateNewWith(
                _yearsNaturalGasSellingPrices,
                _cogenerationParameterService,
                _newNaturalGasSellingPrice,
                _identityFactory);

            _activeCgn.Period.ActiveTill.Should().Be(_newNaturalGasSellingPrice.Period.ActiveFrom);
            newChp.LowerRate.Should().Be(_activeCgn.LowerRate * cogenerationParameter);
            newChp.HigherRate.Should().Be(_activeCgn.HigherRate * cogenerationParameter);
            newChp.NaturalGasSellingPrice.Should().Be(_newNaturalGasSellingPrice);
            newChp.Period.Should().Be(_newNaturalGasSellingPrice.Period);
        }
    }
}