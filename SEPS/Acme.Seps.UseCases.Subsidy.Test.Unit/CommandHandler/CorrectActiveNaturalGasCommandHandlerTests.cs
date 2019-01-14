using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Test.Unit.Utility.Factory;
using Acme.Seps.UseCases.Subsidy.Command;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace Acme.Seps.UseCases.Subsidy.Test.Unit.CommandHandler
{
    public class CorrectActiveNaturalGasCommandHandlerTests
    {
        private readonly ISepsCommandHandler<CorrectActiveNaturalGasSellingPriceCommand> _calculateNaturalGas;
        private readonly IUnitOfWork _unitOfWork;

        public CorrectActiveNaturalGasCommandHandlerTests()
        {
            DateTimeOffset nineMonthsAgo = DateTime.Now.AddMonths(-9);

            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(nineMonthsAgo);
            var activeNgsp = ngspFactory.Create();

            var cogenerationParameterService = Substitute.For<ICogenerationParameterService>();
            cogenerationParameterService
                .Calculate(Arg.Any<AverageElectricEnergyProductionPrice>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(1M);

            ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(nineMonthsAgo.AddMonths(-5));
            var previousActiveNgsp = ngspFactory.Create();

            IEconometricIndexFactory<AverageElectricEnergyProductionPrice> aeeppFactory =
                new EconometricIndexFactory<AverageElectricEnergyProductionPrice>(
                    nineMonthsAgo.ToFirstDayOfTheYear().AddYears(-1));
            var activeAeepp = aeeppFactory.Create();

            ITariffFactory<CogenerationTariff> cogenerationFactory =
                new CogenerationTariffFactory(activeAeepp, previousActiveNgsp);
            var previousActiveCtfs = new List<CogenerationTariff> { cogenerationFactory.Create() };

            cogenerationFactory = new CogenerationTariffFactory(activeAeepp, activeNgsp);
            var activeCtfs = new List<CogenerationTariff> { cogenerationFactory.Create() };

            var dummyGuid = Guid.NewGuid();
            typeof(CogenerationTariff).BaseType
                .GetProperty("ProjectTypeId").SetValue(previousActiveCtfs[0], dummyGuid);
            typeof(CogenerationTariff).BaseType
                .GetProperty("ProjectTypeId").SetValue(activeCtfs[0], dummyGuid);

            var repository = Substitute.For<IRepository>();
            repository
                .GetSingle(Arg.Any<ActiveSpecification<NaturalGasSellingPrice>>())
                .Returns(activeNgsp);
            repository
                .GetSingle(Arg.Any<PreviousActiveSpecification<NaturalGasSellingPrice>>())
                .Returns(previousActiveNgsp);
            repository
                .GetAll(Arg.Any<PreviousActiveSpecification<CogenerationTariff>>())
                .Returns(previousActiveCtfs);
            repository
                .GetAll(Arg.Any<ActiveSpecification<CogenerationTariff>>())
                .Returns(activeCtfs);
            repository
                .GetSingle(Arg.Any<ActiveSpecification<AverageElectricEnergyProductionPrice>>())
                .Returns(activeAeepp);

            _unitOfWork = Substitute.For<IUnitOfWork>();

            _calculateNaturalGas = new CorrectActiveNaturalGasSellingPriceCommandHandler(
                cogenerationParameterService, repository, _unitOfWork, Substitute.For<IIdentityFactory<Guid>>());
        }

        public void ExecutesProperly()
        {
            var lastPeriod = DateTime.Now.AddMonths(-3);

            var correctActiveNaturalGasCommand = new CorrectActiveNaturalGasSellingPriceCommand
            {
                Amount = 100M,
                Month = lastPeriod.Month,
                Remark = nameof(CalculateNaturalGasSellingPriceCommand),
                Year = lastPeriod.Year,
            };

            _calculateNaturalGas.Handle(correctActiveNaturalGasCommand);

            _unitOfWork.Received(2).Update(Arg.Any<CogenerationTariff>());
            _unitOfWork.Received(2).Update(Arg.Any<NaturalGasSellingPrice>());
            _unitOfWork.Received().Commit();
        }
    }
}