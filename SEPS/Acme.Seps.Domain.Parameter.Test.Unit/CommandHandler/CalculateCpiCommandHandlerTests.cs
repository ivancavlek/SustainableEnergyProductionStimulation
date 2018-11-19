using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.CommandHandler;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.CommandHandler
{
    public class CalculateCpiCommandHandlerTests
    {
        private readonly ISepsCommandHandler<CalculateCpiCommand> _calculateCpi;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository _repository;
        private readonly IPeriodFactory _periodFactory;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CalculateCpiCommandHandlerTests()
        {
            _periodFactory = new YearlyPeriodFactory(DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-3));
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _identityFactory.CreateIdentity().Returns(Guid.NewGuid());

            var cpi = Activator.CreateInstance(
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

            var renewableEnergySourceTariff = Activator.CreateInstance(
                    typeof(RenewableEnergySourceTariff),
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[]
                    {
                        cpi,
                        100,
                        500,
                        5M,
                        10M,
                        new YearlyPeriodFactory(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                        _identityFactory
                    },
                    null) as RenewableEnergySourceTariff;
            _repository = Substitute.For<IRepository>();
            _repository
                .GetAll(Arg.Any<BaseSpecification<RenewableEnergySourceTariff>>())
                .Returns(new List<RenewableEnergySourceTariff> { renewableEnergySourceTariff });
            _repository
                .GetSingle(Arg.Any<BaseSpecification<ConsumerPriceIndex>>())
                .Returns(cpi);

            _unitOfWork = Substitute.For<IUnitOfWork>();

            _calculateCpi = new CalculateCpiCommandHandler(_repository, _unitOfWork, _identityFactory);
        }

        public void ExecutesProperly()
        {
            var calculateCommand = new CalculateCpiCommand
            {
                Amount = 100M,
                Remark = nameof(CalculateCpiCommand)
            };

            using (var monitoredEvent = _calculateCpi.Monitor())
            {
                _calculateCpi.Handle(calculateCommand);

                _unitOfWork.Received().Insert(Arg.Any<ConsumerPriceIndex>());
                _unitOfWork.Received().Insert(Arg.Any<RenewableEnergySourceTariff>());
                _unitOfWork.Received().Commit();
                monitoredEvent.Should().Raise("UseCaseExecutionProcessing");
            }
        }
    }
}