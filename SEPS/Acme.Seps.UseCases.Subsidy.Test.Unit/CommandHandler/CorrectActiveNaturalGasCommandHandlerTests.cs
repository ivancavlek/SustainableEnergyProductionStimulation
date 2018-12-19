using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Test.Unit.Utility.Factory;
using Acme.Seps.UseCases.Subsidy.Command;
using Acme.Seps.UseCases.Subsidy.Command.Repository;
using FluentAssertions;
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
                .Calculate(Arg.Any<YearlyAverageElectricEnergyProductionPrice>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(1M);

            ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(nineMonthsAgo.AddMonths(-5));
            var previousActiveNgsp = ngspFactory.Create();

            IEconometricIndexFactory<YearlyAverageElectricEnergyProductionPrice> yeapFactory =
                new EconometricIndexFactory<YearlyAverageElectricEnergyProductionPrice>(
                    nineMonthsAgo.ToFirstDayOfTheYear().AddYears(-1));
            var activeYeap = yeapFactory.Create();

            ITariffFactory<CogenerationTariff> cogenerationFactory =
                new CogenerationTariffFactory(activeYeap, previousActiveNgsp);
            var previousActiveCtfs = new List<CogenerationTariff> { cogenerationFactory.Create() };

            cogenerationFactory = new CogenerationTariffFactory(activeYeap, activeNgsp);
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
                .GetSingle(Arg.Any<ActiveSpecification<YearlyAverageElectricEnergyProductionPrice>>())
                .Returns(activeYeap);

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

            using (var monitoredEvent = _calculateNaturalGas.Monitor())
            {
                _calculateNaturalGas.Handle(correctActiveNaturalGasCommand);

                _unitOfWork.Received(2).Update(Arg.Any<CogenerationTariff>());
                _unitOfWork.Received(2).Update(Arg.Any<NaturalGasSellingPrice>());
                _unitOfWork.Received().Commit();
                monitoredEvent.Should().Raise("UseCaseExecutionProcessing");
            }
        }
    }
}