using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Subsidy.Command;
using Acme.Seps.Domain.Subsidy.CommandHandler;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.CommandHandler
{
    public class CorrectActiveNaturalGasCommandHandlerTests
    {
        private readonly ISepsCommandHandler<CorrectActiveNaturalGasCommand> _calculateNaturalGas;
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISepsRepository _repository;
        private readonly IIdentityFactory<Guid> _identityFactory;

        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly List<CogenerationTariff> _cogenerationTariffs;
        private readonly DateTime _lastPeriod;

        public CorrectActiveNaturalGasCommandHandlerTests()
        {
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _identityFactory.CreateIdentity().Returns(Guid.NewGuid());

            _lastPeriod = DateTime.Now.AddMonths(-3);

            _cogenerationParameterService = Substitute.For<ICogenerationParameterService>();
            _cogenerationParameterService
                .GetFrom(Arg.Any<IEnumerable<NaturalGasSellingPrice>>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(1M);

            _naturalGasSellingPrice = Activator.CreateInstance(
                typeof(NaturalGasSellingPrice),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[]
                {
                    10M,
                    nameof(NaturalGasSellingPrice),
                    new Period(new MonthlyPeriodFactory(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2))),
                    _identityFactory
                },
                null) as NaturalGasSellingPrice;

            var previousCogenerationTariffs = new List<CogenerationTariff> { Activator.CreateInstance(
                    typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[]
                {
                    _naturalGasSellingPrice,
                    100,
                    500,
                    10M,
                    10M,
                    Guid.NewGuid(),
                    new MonthlyPeriodFactory(DateTime.Now.AddMonths(-4), _lastPeriod),
                    _identityFactory
                },
                null) as CogenerationTariff };

            _cogenerationTariffs = new List<CogenerationTariff> { Activator.CreateInstance(
                    typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[]
                {
                    _naturalGasSellingPrice,
                    100,
                    500,
                    10M,
                    10M,
                    Guid.NewGuid(),
                    new MonthlyPeriodFactory(DateTime.Now.AddMonths(-4), _lastPeriod),
                    _identityFactory
                },
                null) as CogenerationTariff };

            var dummyGuid = Guid.NewGuid();
            typeof(CogenerationTariff).BaseType
                .GetProperty("ProjectTypeId").SetValue(previousCogenerationTariffs[0], dummyGuid);
            typeof(CogenerationTariff).BaseType
                .GetProperty("ProjectTypeId").SetValue(_cogenerationTariffs[0], dummyGuid);

            _repository = Substitute.For<ISepsRepository>();
            _repository.GetLatest<NaturalGasSellingPrice>().Returns(_naturalGasSellingPrice);
            _repository
                .GetAll(Arg.Any<BaseSpecification<CogenerationTariff>>())
                .Returns(previousCogenerationTariffs, _cogenerationTariffs);

            _unitOfWork = Substitute.For<IUnitOfWork>();

            _calculateNaturalGas = new CorrectActiveNaturalGasCommandHandler(
                _cogenerationParameterService, _repository, _unitOfWork, _identityFactory);
        }

        public void ExecutesProperly()
        {
            var correctActiveNaturalGasCommand = new CorrectActiveNaturalGasCommand
            {
                Amount = 100M,
                Month = _lastPeriod.Month,
                Remark = nameof(CalculateNaturalGasCommand),
                Year = _lastPeriod.Year,
            };

            using (var monitoredEvent = _calculateNaturalGas.Monitor())
            {
                _calculateNaturalGas.Handle(correctActiveNaturalGasCommand);

                _unitOfWork.Received().Update(Arg.Any<CogenerationTariff>());
                _unitOfWork.Received().Commit();
                monitoredEvent.Should().Raise("UseCaseExecutionProcessing");
            }
        }
    }
}