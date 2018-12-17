using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Infrastructure;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.UseCases.Subsidy.Command.DomainService;
using Acme.Seps.UseCases.Subsidy.Command.Entity;
using Acme.Seps.UseCases.Subsidy.Command.Infrastructure;
using Acme.Seps.Domain.Subsidy.Test.Unit.Factory;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reflection;

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

            DateTimeOffset cgnSince = DateTimeOffset.Now.ToFirstDayOfTheMonth().AddMonths(-9);
            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(cgnSince);
            ITariffFactory<CogenerationTariff> cogenerationFactory =
                new TariffFactory<CogenerationTariff>(ngspFactory.Create());
            _activeCgn = cogenerationFactory.Create();

            DateTimeOffset ngspSince = DateTimeOffset.Now.ToFirstDayOfTheMonth().AddMonths(-4);
            ngspFactory = new EconometricIndexFactory<NaturalGasSellingPrice>(ngspSince);
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
            typeof(NaturalGasSellingPrice)
                .BaseType.BaseType.BaseType
                .GetMethod("SetInactive", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(_newNaturalGasSellingPrice, new object[] { DateTimeOffset.Now });

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

            _activeCgn.Active.Until.Should().Be(_newNaturalGasSellingPrice.Active.Since);
            newChp.LowerRate.Should().Be(_activeCgn.LowerRate * cogenerationParameter);
            newChp.HigherRate.Should().Be(_activeCgn.HigherRate * cogenerationParameter);
            newChp.NaturalGasSellingPrice.Should().Be(_newNaturalGasSellingPrice);
            newChp.Active.Should().Be(_newNaturalGasSellingPrice.Active);
        }
    }
}