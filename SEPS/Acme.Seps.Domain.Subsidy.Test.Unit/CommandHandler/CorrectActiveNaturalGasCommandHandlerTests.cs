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
    public class CorrectActiveNaturalGasCommandHandlerTests
    {
        private readonly ISepsCommandHandler<CorrectActiveNaturalGasSellingPriceCommand> _calculateNaturalGas;
        private readonly IUnitOfWork _unitOfWork;

        public CorrectActiveNaturalGasCommandHandlerTests()
        {
            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(DateTime.Now.AddMonths(-9));
            var activeNgsp = ngspFactory.Create();

            var cogenerationParameterService = Substitute.For<ICogenerationParameterService>();
            cogenerationParameterService
                .GetFrom(Arg.Any<IEnumerable<NaturalGasSellingPrice>>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(1M);

            ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(activeNgsp.Active.Since.AddMonths(-5));
            var previousActiveNgsp = ngspFactory.Create();

            ITariffFactory<CogenerationTariff> cogenerationFactory = new TariffFactory<CogenerationTariff>(
                previousActiveNgsp);
            var previousActiveCtfs = new List<CogenerationTariff> { cogenerationFactory.Create() };

            cogenerationFactory = new TariffFactory<CogenerationTariff>(activeNgsp);
            var activeCtfs = new List<CogenerationTariff> { cogenerationFactory.Create() };

            var dummyGuid = Guid.NewGuid();
            typeof(CogenerationTariff).BaseType
                .GetProperty("ProjectTypeId").SetValue(previousActiveCtfs[0], dummyGuid);
            typeof(CogenerationTariff).BaseType
                .GetProperty("ProjectTypeId").SetValue(activeCtfs[0], dummyGuid);

            var repository = Substitute.For<IRepository>();
            repository.GetSingle(Arg.Any<ActiveSpecification<NaturalGasSellingPrice>>()).Returns(activeNgsp);
            repository
                .GetSingle(Arg.Any<PreviousActiveSpecification<NaturalGasSellingPrice>>())
                .Returns(previousActiveNgsp);
            repository
                .GetAll(Arg.Any<PreviousActiveSpecification<CogenerationTariff>>())
                .Returns(previousActiveCtfs);
            repository
                .GetAll(Arg.Any<ActiveSpecification<CogenerationTariff>>())
                .Returns(activeCtfs);

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

                _unitOfWork.Received().Update(Arg.Any<NaturalGasSellingPrice>());
                _unitOfWork.Received().Update(Arg.Any<NaturalGasSellingPrice>());
                _unitOfWork.Received().Update(Arg.Any<CogenerationTariff>());
                _unitOfWork.Received().Update(Arg.Any<CogenerationTariff>());
                _unitOfWork.Received().Commit();
                monitoredEvent.Should().Raise("UseCaseExecutionProcessing");
            }
        }
    }
}