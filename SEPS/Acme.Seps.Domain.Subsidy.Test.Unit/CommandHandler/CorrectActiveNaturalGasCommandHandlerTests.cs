using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Subsidy.Command;
using Acme.Seps.Domain.Subsidy.CommandHandler;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Repository;
using Acme.Seps.Domain.Subsidy.Test.Unit.Factory;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.CommandHandler
{
    public class CorrectActiveNaturalGasCommandHandlerTests
    {
        private readonly ISepsCommandHandler<CorrectActiveNaturalGasCommand> _calculateNaturalGas;
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

            ITariffFactory<CogenerationTariff> cogenerationFactory = new TariffFactory<CogenerationTariff>(
                activeNgsp, activeNgsp.Period.ActiveFrom.AddMonths(-5));
            var previousCtfs = new List<CogenerationTariff> { cogenerationFactory.Create() };

            cogenerationFactory = new TariffFactory<CogenerationTariff>(
                activeNgsp, activeNgsp.Period.ActiveFrom);
            var activeCtfs = new List<CogenerationTariff> { cogenerationFactory.Create() };

            var dummyGuid = Guid.NewGuid();
            typeof(CogenerationTariff).BaseType
                .GetProperty("ProjectTypeId").SetValue(previousCtfs[0], dummyGuid);
            typeof(CogenerationTariff).BaseType
                .GetProperty("ProjectTypeId").SetValue(activeCtfs[0], dummyGuid);

            var repository = Substitute.For<IRepository>();
            repository.GetSingle(Arg.Any<ActiveSpecification<NaturalGasSellingPrice>>()).Returns(activeNgsp);
            repository
                .GetAll(Arg.Any<PreviousActiveSpecification<CogenerationTariff>>())
                .Returns(previousCtfs);
            repository
                .GetAll(Arg.Any<NgspCogenerationTariffSpecification>())
                .Returns(activeCtfs);

            _unitOfWork = Substitute.For<IUnitOfWork>();

            _calculateNaturalGas = new CorrectActiveNaturalGasCommandHandler(
                cogenerationParameterService, repository, _unitOfWork, Substitute.For<IIdentityFactory<Guid>>());
        }

        public void ExecutesProperly()
        {
            var lastPeriod = DateTime.Now.AddMonths(-3);

            var correctActiveNaturalGasCommand = new CorrectActiveNaturalGasCommand
            {
                Amount = 100M,
                Month = lastPeriod.Month,
                Remark = nameof(CalculateNaturalGasCommand),
                Year = lastPeriod.Year,
            };

            using (var monitoredEvent = _calculateNaturalGas.Monitor())
            {
                _calculateNaturalGas.Handle(correctActiveNaturalGasCommand);

                _unitOfWork.Received().Update(Arg.Any<NaturalGasSellingPrice>());
                _unitOfWork.Received().Update(Arg.Any<CogenerationTariff>());
                _unitOfWork.Received().Commit();
                monitoredEvent.Should().Raise("UseCaseExecutionProcessing");
            }
        }
    }
}