using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Test.Unit.Utility.Factory;
using Acme.Seps.Text;
using FluentAssertions;
using NSubstitute;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class CogenerationTariffTests
    {
        private readonly CogenerationTariff _activeCgn;
        private readonly NaturalGasSellingPrice _newNaturalGasSellingPrice;
        private readonly AverageElectricEnergyProductionPrice _averageElectricEnergyProductionPrice;
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CogenerationTariffTests()
        {
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _cogenerationParameterService = Substitute.For<ICogenerationParameterService>();

            DateTimeOffset cgnSince = DateTimeOffset.Now.ToFirstDayOfTheMonth().AddMonths(-9);

            IEconometricIndexFactory<AverageElectricEnergyProductionPrice> aeeppFactory =
                new EconometricIndexFactory<AverageElectricEnergyProductionPrice>(
                    cgnSince.AddYears(-1).ToFirstDayOfTheYear());
            _averageElectricEnergyProductionPrice = aeeppFactory.Create();

            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(cgnSince);
            ITariffFactory<CogenerationTariff> cogenerationFactory =
                new CogenerationTariffFactory(_averageElectricEnergyProductionPrice, ngspFactory.Create());
            _activeCgn = cogenerationFactory.Create();

            DateTimeOffset ngspSince = DateTimeOffset.Now.ToFirstDayOfTheMonth().AddMonths(-4);
            ngspFactory = new EconometricIndexFactory<NaturalGasSellingPrice>(ngspSince);
            _newNaturalGasSellingPrice = ngspFactory.Create();
        }

        public void CogenerationParameterServiceMustBeSet()
        {
            Action action = () => _activeCgn.CreateNewWith(
                null, _averageElectricEnergyProductionPrice, _newNaturalGasSellingPrice, _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentNullException>()
                .WithMessage(SepsMessage.EntityNotSet("cogenerationParameterService"));
        }

        public void AverageElectricEnergyProductionPriceMustBeSet()
        {
            Action action = () => _activeCgn.CreateNewWith(
                _cogenerationParameterService, null, _newNaturalGasSellingPrice, _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentNullException>()
                .WithMessage(SepsMessage.EntityNotSet("averageElectricEnergyProductionPrice"));
        }

        public void AverageElectricEnergyProductionPriceMustBeActive()
        {
            typeof(AverageElectricEnergyProductionPrice)
                .BaseType.BaseType.BaseType
                .GetMethod("SetInactive", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(_averageElectricEnergyProductionPrice, new object[] { DateTimeOffset.Now });

            Action action = () => _activeCgn.CreateNewWith(
                _cogenerationParameterService,
                _averageElectricEnergyProductionPrice,
                _newNaturalGasSellingPrice,
                _identityFactory);

            action
                .Should()
                .Throw<Exception>()
                .WithMessage(SepsMessage.InactiveException("averageElectricEnergyProductionPrice"));
        }

        public void NaturalGasSellingPriceMustBeSet()
        {
            Action action = () => _activeCgn.CreateNewWith(
                _cogenerationParameterService, _averageElectricEnergyProductionPrice, null, _identityFactory);

            action
                .Should()
                .ThrowExactly<ArgumentNullException>()
                .WithMessage(SepsMessage.EntityNotSet("naturalGasSellingPrice"));
        }

        public void NaturalGasSellingPriceMustBeActive()
        {
            typeof(NaturalGasSellingPrice)
                .BaseType.BaseType.BaseType
                .GetMethod("SetInactive", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(_newNaturalGasSellingPrice, new object[] { DateTimeOffset.Now });

            Action action = () => _activeCgn.CreateNewWith(
                _cogenerationParameterService,
                _averageElectricEnergyProductionPrice,
                _newNaturalGasSellingPrice,
                _identityFactory);

            action
                .Should()
                .Throw<Exception>()
                .WithMessage(SepsMessage.InactiveException("naturalGasSellingPrice"));
        }

        public void CreatesProperly()
        {
            const decimal cogenerationParameter = 1M;

            _cogenerationParameterService
                .Calculate(Arg.Any<AverageElectricEnergyProductionPrice>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(cogenerationParameter);

            var newChp = _activeCgn.CreateNewWith(
                _cogenerationParameterService,
                _averageElectricEnergyProductionPrice,
                _newNaturalGasSellingPrice,
                _identityFactory);

            _activeCgn.Active.Until.Should().Be(_newNaturalGasSellingPrice.Active.Since);
            newChp.LowerRate.Should().Be(_activeCgn.LowerRate * cogenerationParameter);
            newChp.HigherRate.Should().Be(_activeCgn.HigherRate * cogenerationParameter);
            newChp.NaturalGasSellingPrice.Should().Be(_newNaturalGasSellingPrice);
            newChp.Active.Should().Be(_newNaturalGasSellingPrice.Active);
        }

        public void NaturalGasSellingPriceIsCorrected()
        {
            const decimal cogenerationParameter = 1M;

            _cogenerationParameterService
                .Calculate(Arg.Any<AverageElectricEnergyProductionPrice>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(cogenerationParameter);

            DateTimeOffset cgnSince = DateTimeOffset.Now.ToFirstDayOfTheMonth().AddMonths(-9);
            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(cgnSince);
            ITariffFactory<CogenerationTariff> cogenerationFactory =
                new CogenerationTariffFactory(_averageElectricEnergyProductionPrice, ngspFactory.Create());
            var previousCgn = cogenerationFactory.Create();
            ngspFactory = new EconometricIndexFactory<NaturalGasSellingPrice>(cgnSince.AddMonths(6));
            var correctedNgsp = ngspFactory.Create();

            _activeCgn.NaturalGasSellingPriceCorrection(
                _cogenerationParameterService, _averageElectricEnergyProductionPrice, correctedNgsp, previousCgn);

            _activeCgn.LowerRate.Should().Be(_activeCgn.LowerRate * cogenerationParameter);
            _activeCgn.HigherRate.Should().Be(_activeCgn.HigherRate * cogenerationParameter);
            _activeCgn.Active.Since.Should().Be(correctedNgsp.Active.Since);
            previousCgn.Active.Until.Should().Be(correctedNgsp.Active.Since);
        }
    }
}