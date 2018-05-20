using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ApplicationService;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.CommandHandler;
using Acme.Seps.Domain.Parameter.Entity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.CommandHandler
{
    public class CalculateCpiCommandHandlerTests
    {
        private readonly ICommandHandler<CalculateCpiCommand> _calculateCpi;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository _repository;
        private readonly IIdentityFactory<Guid> _identityFactory;
        private readonly ISepsLogService _sepsLogService;

        public CalculateCpiCommandHandlerTests()
        {
            _sepsLogService = Substitute.For<ISepsLogService>();
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _identityFactory.CreateIdentity().Returns(Guid.NewGuid());

            var _cpi = Activator.CreateInstance(
                typeof(ConsumerPriceIndex),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    10M,
                    nameof(ConsumerPriceIndex),
                    new YearlyPeriod(DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-3)),
                    _identityFactory },
                null) as ConsumerPriceIndex;

            var renewableEnergySourceTariff = Activator.CreateInstance(
                    typeof(RenewableEnergySourceTariff),
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { _cpi, 5M, 10M, _identityFactory },
                    null) as RenewableEnergySourceTariff;
            _repository = Substitute.For<IRepository>();
            _repository
                .GetAll(Arg.Any<ISpecification<RenewableEnergySourceTariff>>())
                .Returns(new List<RenewableEnergySourceTariff> { renewableEnergySourceTariff });
            _repository
                .GetSingle(Arg.Any<ISpecification<ConsumerPriceIndex>>())
                .Returns(_cpi);

            _unitOfWork = Substitute.For<IUnitOfWork>();

            _calculateCpi = new CalculateCpiCommandHandler(
                _repository, _unitOfWork, _identityFactory, _sepsLogService);
        }

        public void ExecutesProperly()
        {
            var calculateCommand = new CalculateCpiCommand
            {
                Amount = 100M,
                Remark = nameof(CalculateCpiCommand)
            };

            _calculateCpi.Handle(calculateCommand);

            _unitOfWork.Received().Insert(Arg.Any<ConsumerPriceIndex>());
            _unitOfWork.Received().Insert(Arg.Any<RenewableEnergySourceTariff>());
            _unitOfWork.Received().Commit();
            _sepsLogService.Received(3).Log(Arg.Any<EntityExecutionLoggingEventArgs>());
        }
    }
}