using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Subsidy.Test.Unit.Factory;
using Acme.Seps.UseCases.Subsidy.Command;
using Acme.Seps.UseCases.Subsidy.Command.DomainService;
using Acme.Seps.UseCases.Subsidy.Command.Entity;
using Acme.Seps.UseCases.Subsidy.Command.Repository;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.CommandHandler
{
    public class CalculateNaturalGasCommandHandlerTests
    {
        private readonly ISepsCommandHandler<CalculateNaturalGasSellingPriceCommand> _calculateNaturalGas;
        private readonly IUnitOfWork _unitOfWork;

        public CalculateNaturalGasCommandHandlerTests()
        {
            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(DateTime.Now.AddMonths(-9));
            var activeNgsp = ngspFactory.Create();

            ITariffFactory<CogenerationTariff> cogenerationFactory = new TariffFactory<CogenerationTariff>(activeNgsp);
            var activeCogenerationTariff = cogenerationFactory.Create();

            var repository = Substitute.For<IRepository>();
            repository
                .GetSingle(Arg.Any<ActiveSpecification<NaturalGasSellingPrice>>())
                .Returns(activeNgsp);
            repository
                .GetAll(Arg.Any<ActiveSpecification<CogenerationTariff>>())
                .Returns(new List<CogenerationTariff> { activeCogenerationTariff });
            repository
                .GetAll(Arg.Any<NaturalGasSellingPricesInAYearSpecification>())
                .Returns(new List<NaturalGasSellingPrice> { activeNgsp });

            _unitOfWork = Substitute.For<IUnitOfWork>();

            var cogenerationParameterService = Substitute.For<ICogenerationParameterService>();
            cogenerationParameterService
                .GetFrom(Arg.Any<IEnumerable<NaturalGasSellingPrice>>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(1M);

            _calculateNaturalGas = new CalculateNaturalGasSellingPriceCommandHandler(
                cogenerationParameterService, repository, _unitOfWork, Substitute.For<IIdentityFactory<Guid>>());
        }

        public void ExecutesProperly()
        {
            var lastPeriod = DateTime.Now.AddMonths(-3);

            var calculateNaturalGasCommand = new CalculateNaturalGasSellingPriceCommand
            {
                Amount = 100M,
                Month = lastPeriod.Month,
                Remark = nameof(CalculateNaturalGasSellingPriceCommand),
                Year = lastPeriod.Year,
            };

            using (var monitoredEvent = _calculateNaturalGas.Monitor())
            {
                _calculateNaturalGas.Handle(calculateNaturalGasCommand);

                _unitOfWork.Received().Update(Arg.Any<NaturalGasSellingPrice>());
                _unitOfWork.Received().Insert(Arg.Any<NaturalGasSellingPrice>());
                _unitOfWork.Received().Update(Arg.Any<CogenerationTariff>());
                _unitOfWork.Received().Insert(Arg.Any<CogenerationTariff>());
                _unitOfWork.Received().Commit();
                monitoredEvent.Should().Raise("UseCaseExecutionProcessing");
            }
        }
    }
}