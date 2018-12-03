using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Subsidy.Command;
using Acme.Seps.Domain.Subsidy.CommandHandler;
using Acme.Seps.Domain.Subsidy.Entity;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.CommandHandler
{
    public class CorrectActiveCpiCommandHandlerTests
    {
        private readonly ISepsCommandHandler<CorrectActiveCpiCommand> _correctActiveCpi;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISepsRepository _repository;
        private readonly IIdentityFactory<Guid> _identityFactory;

        private readonly ConsumerPriceIndex _cpi;
        private readonly List<RenewableEnergySourceTariff> _resTariffs;

        public CorrectActiveCpiCommandHandlerTests()
        {
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _identityFactory.CreateIdentity().Returns(Guid.NewGuid());

            _cpi = Activator.CreateInstance(
                typeof(ConsumerPriceIndex),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[]
                {
                    10M,
                    nameof(ConsumerPriceIndex),
                    new Period(new YearlyPeriodFactory(DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-3))),
                    _identityFactory
                },
                null) as ConsumerPriceIndex;

            var previousResTariffs = new List<RenewableEnergySourceTariff> { Activator.CreateInstance(
                typeof(RenewableEnergySourceTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[]
                {
                    _cpi,
                    100,
                    500,
                    5M,
                    10M,
                    Guid.NewGuid(),
                    new YearlyPeriodFactory(DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-3)),
                    _identityFactory
                },
                null) as RenewableEnergySourceTariff };

            _resTariffs = new List<RenewableEnergySourceTariff> { Activator.CreateInstance(
                typeof(RenewableEnergySourceTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[]
                {
                    _cpi,
                    100,
                    500,
                    5M,
                    10M,
                    Guid.NewGuid(),
                    new YearlyPeriodFactory(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                    _identityFactory
                },
                null) as RenewableEnergySourceTariff };

            var dummyGuid = Guid.NewGuid();
            typeof(RenewableEnergySourceTariff).BaseType
                .GetProperty("ProjectTypeId").SetValue(previousResTariffs[0], dummyGuid);
            typeof(RenewableEnergySourceTariff).BaseType
                .GetProperty("ProjectTypeId").SetValue(_resTariffs[0], dummyGuid);

            _repository = Substitute.For<ISepsRepository>();
            _repository.GetLatest<ConsumerPriceIndex>().Returns(_cpi);
            _repository
                .GetAll(Arg.Any<BaseSpecification<RenewableEnergySourceTariff>>())
                .Returns(previousResTariffs, _resTariffs);

            _unitOfWork = Substitute.For<IUnitOfWork>();

            _correctActiveCpi = new CorrectActiveCpiCommandHandler(_repository, _unitOfWork, _identityFactory);
        }

        public void ExecutesProperly()
        {
            var correctActiveCpi = new CorrectActiveCpiCommand
            {
                Amount = 100M,
                Remark = nameof(CalculateCpiCommand)
            };

            using (var monitoredEvent = _correctActiveCpi.Monitor())
            {
                _correctActiveCpi.Handle(correctActiveCpi);

                _unitOfWork.Received().Update(Arg.Any<RenewableEnergySourceTariff>());
                _unitOfWork.Received().Commit();
                monitoredEvent.Should().Raise("UseCaseExecutionProcessing");
            }
        }
    }
}