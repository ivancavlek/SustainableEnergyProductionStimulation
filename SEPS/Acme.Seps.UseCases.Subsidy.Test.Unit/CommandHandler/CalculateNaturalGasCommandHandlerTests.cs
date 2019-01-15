using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
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
    public class CalculateNaturalGasCommandHandlerTests
    {
        private readonly ISepsCommandHandler<CalculateNewNaturalGasSellingPriceCommand> _calculateNaturalGas;
        private readonly IUnitOfWork _unitOfWork;

        public CalculateNaturalGasCommandHandlerTests()
        {
            DateTimeOffset nineMonthsAgo = DateTime.Now.AddMonths(-9);

            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(nineMonthsAgo);
            var activeNgsp = ngspFactory.Create();

            IEconometricIndexFactory<AverageElectricEnergyProductionPrice> aeeppFactory =
                new EconometricIndexFactory<AverageElectricEnergyProductionPrice>(
                    nineMonthsAgo.ToFirstDayOfTheYear().AddYears(-1));
            var activeAeepp = aeeppFactory.Create();

            ITariffFactory<CogenerationTariff> cogenerationFactory =
                new CogenerationTariffFactory(activeAeepp, activeNgsp);
            var activeCogenerationTariff = cogenerationFactory.Create();

            var repository = Substitute.For<IRepository>();
            repository
                .GetSingle(Arg.Any<ActiveSpecification<NaturalGasSellingPrice>>())
                .Returns(activeNgsp);
            repository
                .GetAll(Arg.Any<ActiveSpecification<CogenerationTariff>>())
                .Returns(new List<CogenerationTariff> { activeCogenerationTariff });
            repository
                .GetSingle(Arg.Any<ActiveSpecification<AverageElectricEnergyProductionPrice>>())
                .Returns(activeAeepp);

            _unitOfWork = Substitute.For<IUnitOfWork>();

            var cogenerationParameterService = Substitute.For<ICogenerationParameterService>();
            cogenerationParameterService
                .Calculate(Arg.Any<AverageElectricEnergyProductionPrice>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(1M);

            _calculateNaturalGas = new CalculateNaturalGasSellingPriceCommandHandler(
                cogenerationParameterService, repository, _unitOfWork, Substitute.For<IIdentityFactory<Guid>>());
        }

        public void ExecutesProperly()
        {
            var lastPeriod = DateTime.Now.AddMonths(-3);

            var calculateNaturalGasCommand = new CalculateNewNaturalGasSellingPriceCommand
            {
                Amount = 100M,
                Month = lastPeriod.Month,
                Remark = nameof(CalculateNewNaturalGasSellingPriceCommand),
                Year = lastPeriod.Year,
            };

            _calculateNaturalGas.Handle(calculateNaturalGasCommand);

            _unitOfWork.Received().Update(Arg.Any<CogenerationTariff>());
            _unitOfWork.Received().Insert(Arg.Any<CogenerationTariff>());
            _unitOfWork.Received().Update(Arg.Any<NaturalGasSellingPrice>());
            _unitOfWork.Received().Insert(Arg.Any<NaturalGasSellingPrice>());
            _unitOfWork.Received().Commit();
        }
    }
}