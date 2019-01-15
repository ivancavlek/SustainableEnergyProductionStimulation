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
    public class CorrectActiveAverageElectricEnergyProductionPriceCommandHandlerTests
    {
        private readonly ISepsCommandHandler<CorrectActiveAverageElectricEnergyProductionPriceCommand> _correctActiveAeepp;
        private readonly IUnitOfWork _unitOfWork;

        public CorrectActiveAverageElectricEnergyProductionPriceCommandHandlerTests()
        {
            DateTimeOffset nineMonthsAgo = DateTime.Now.AddMonths(-9);

            IEconometricIndexFactory<AverageElectricEnergyProductionPrice> aeeppFactory =
                new EconometricIndexFactory<AverageElectricEnergyProductionPrice>(nineMonthsAgo);
            var activeAeepp = aeeppFactory.Create();

            var cogenerationParameterService = Substitute.For<ICogenerationParameterService>();
            cogenerationParameterService
                .Calculate(Arg.Any<AverageElectricEnergyProductionPrice>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(1M);

            aeeppFactory =
                new EconometricIndexFactory<AverageElectricEnergyProductionPrice>(nineMonthsAgo.AddMonths(-5));
            var previousActiveAeepp = aeeppFactory.Create();

            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(
                    nineMonthsAgo.ToFirstDayOfTheYear().AddYears(-1));
            var activeNgsp = ngspFactory.Create();

            ITariffFactory<CogenerationTariff> cogenerationFactory =
                new CogenerationTariffFactory(previousActiveAeepp, activeNgsp);
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
                .GetSingle(Arg.Any<ActiveSpecification<AverageElectricEnergyProductionPrice>>())
                .Returns(activeAeepp);
            repository
                .GetSingle(Arg.Any<PreviousActiveSpecification<AverageElectricEnergyProductionPrice>>())
                .Returns(previousActiveAeepp);
            repository
                .GetAll(Arg.Any<PreviousActiveSpecification<CogenerationTariff>>())
                .Returns(previousActiveCtfs);
            repository
                .GetAll(Arg.Any<ActiveSpecification<CogenerationTariff>>())
                .Returns(activeCtfs);
            repository
                .GetSingle(Arg.Any<ActiveSpecification<NaturalGasSellingPrice>>())
                .Returns(activeNgsp);

            _unitOfWork = Substitute.For<IUnitOfWork>();

            _correctActiveAeepp = new CorrectActiveAverageElectricEnergyProductionPriceCommandHandler(
                cogenerationParameterService, repository, _unitOfWork, Substitute.For<IIdentityFactory<Guid>>());
        }

        public void ExecutesProperly()
        {
            var lastPeriod = DateTime.Now.AddMonths(-3);

            var correctActiveNaturalGasCommand = new CorrectActiveAverageElectricEnergyProductionPriceCommand
            {
                Amount = 100M,
                Month = lastPeriod.Month,
                Remark = nameof(CorrectActiveAverageElectricEnergyProductionPriceCommand),
                Year = lastPeriod.Year,
            };

            _correctActiveAeepp.Handle(correctActiveNaturalGasCommand);

            _unitOfWork.Received(2).Update(Arg.Any<CogenerationTariff>());
            _unitOfWork.Received(2).Update(Arg.Any<AverageElectricEnergyProductionPrice>());
            _unitOfWork.Received().Commit();
        }
    }
}