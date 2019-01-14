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
    public class CalculateAverageElectricEnergyProductionPriceTests
    {
        private readonly ISepsCommandHandler<CalculateNewAverageElectricEnergyProductionPriceCommand> _calculateNewAverageElectricEnergyProductionPrice;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository _repository;

        private DateTimeOffset _period;

        public CalculateAverageElectricEnergyProductionPriceTests()
        {
            _period = DateTime.Now.AddYears(-1).AddMonths(-9);

            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(_period);
            var activeNgsp = ngspFactory.Create();

            IEconometricIndexFactory<AverageElectricEnergyProductionPrice> aeeppFactory =
                new EconometricIndexFactory<AverageElectricEnergyProductionPrice>(_period.ToFirstDayOfTheYear());
            var activeAeepp = aeeppFactory.Create();

            ITariffFactory<CogenerationTariff> cogenerationFactory =
                new CogenerationTariffFactory(activeAeepp, activeNgsp);
            var activeCogenerationTariff = cogenerationFactory.Create();

            _repository = Substitute.For<IRepository>();
            _repository
                .GetSingle(Arg.Any<ActiveSpecification<NaturalGasSellingPrice>>())
                .Returns(activeNgsp);
            _repository
                .GetAll(Arg.Any<ActiveSpecification<CogenerationTariff>>())
                .Returns(new List<CogenerationTariff> { activeCogenerationTariff });
            _repository
                .GetSingle(Arg.Any<ActiveSpecification<AverageElectricEnergyProductionPrice>>())
                .Returns(activeAeepp);

            _unitOfWork = Substitute.For<IUnitOfWork>();

            var cogenerationParameterService = Substitute.For<ICogenerationParameterService>();
            cogenerationParameterService
                .Calculate(Arg.Any<AverageElectricEnergyProductionPrice>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(1M);

            _calculateNewAverageElectricEnergyProductionPrice =
                    new CalculateNewAverageElectricEnergyProductionPriceCommandHandler(
                        cogenerationParameterService,
                        _repository,
                        _unitOfWork,
                        Substitute.For<IIdentityFactory<Guid>>());
        }

        public void ExecutesProperly()
        {
            var calculateNaturalGasCommand = new CalculateNewAverageElectricEnergyProductionPriceCommand
            {
                Amount = 100M,
                Month = _period.Month,
                Remark = nameof(CalculateNewAverageElectricEnergyProductionPriceCommand),
                Year = _period.Year,
            };

            _calculateNewAverageElectricEnergyProductionPrice.Handle(calculateNaturalGasCommand);

            _unitOfWork.Received().Update(Arg.Any<AverageElectricEnergyProductionPrice>());
            _unitOfWork.Received().Insert(Arg.Any<AverageElectricEnergyProductionPrice>());
            _unitOfWork.Received().Update(Arg.Any<CogenerationTariff>());
            _unitOfWork.Received().Insert(Arg.Any<CogenerationTariff>());
            _unitOfWork.Received().Commit();
        }
    }
}