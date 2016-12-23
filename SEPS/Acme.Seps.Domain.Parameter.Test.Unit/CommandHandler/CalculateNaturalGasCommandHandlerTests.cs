using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ApplicationService;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.CommandHandler;
using Acme.Seps.Domain.Parameter.DomainService;
using Acme.Seps.Domain.Parameter.Entity;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.CommandHandler
{
    public class CalculateNaturalGasCommandHandlerTests
    {
        private readonly ICommandHandler<CalculateNaturalGasCommand> _calculateNaturalGas;
        private readonly Mock<ICogenerationParameterService> _cogenerationParameterService;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;
        private readonly Mock<ISepsLogService> _sepsLogService;

        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly IEnumerable<CogenerationTariff> _cogenerationTariff;
        private readonly DateTime _lastPeriod;

        public CalculateNaturalGasCommandHandlerTests()
        {
            _sepsLogService = new Mock<ISepsLogService>();
            _identityFactory = new Mock<IIdentityFactory<Guid>>();
            _identityFactory.Setup(m => m.CreateIdentity()).Returns(Guid.NewGuid());

            _lastPeriod = DateTime.Now.AddMonths(-3);

            _cogenerationParameterService = new Mock<ICogenerationParameterService>();
            _cogenerationParameterService
                .Setup(m => m.GetFrom(It.IsAny<IEnumerable<NaturalGasSellingPrice>>(), It.IsAny<NaturalGasSellingPrice>()))
                .Returns(1M);

            _naturalGasSellingPrice = Activator.CreateInstance(
                typeof(NaturalGasSellingPrice),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    10M,
                    nameof(NaturalGasSellingPrice),
                    new MonthlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                    _identityFactory.Object },
                null) as NaturalGasSellingPrice;

            _cogenerationTariff = new List<CogenerationTariff> { Activator.CreateInstance(
                    typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    _naturalGasSellingPrice,
                    10M,
                    10M,
                    new MonthlyPeriod(DateTime.Now.AddMonths(-4), _lastPeriod),
                    _identityFactory.Object },
                null) as CogenerationTariff };

            _unitOfWork = new Mock<IUnitOfWork>();

            _calculateNaturalGas = new CalculateNaturalGasCommandHandler(
                _cogenerationParameterService.Object,
                _unitOfWork.Object,
                _identityFactory.Object,
                _sepsLogService.Object);
        }

        public void ExecutesProperly()
        {
            var calculateNaturalGasCommand = new CalculateNaturalGasCommand
            {
                ActiveNaturalGasSellingPrice = _naturalGasSellingPrice,
                ActiveCogenerationTariffs = _cogenerationTariff,
                Amount = 100M,
                Month = _lastPeriod.Month,
                Remark = nameof(CalculateNaturalGasCommand),
                Year = _lastPeriod.Year,
                YearsNaturalGasSellingPrices = new List<NaturalGasSellingPrice> { _naturalGasSellingPrice }
            };

            _calculateNaturalGas.Handle(calculateNaturalGasCommand);

            _unitOfWork.Verify(m => m.Insert(It.IsAny<NaturalGasSellingPrice>()), Times.Once);
            _unitOfWork.Verify(m => m.Insert(It.IsAny<CogenerationTariff>()), Times.Once);
            _unitOfWork.Verify(m => m.Commit(), Times.Once);
            _sepsLogService.Verify(m => m.Log(It.IsAny<EntityExecutionLoggingEventArgs>()), Times.Exactly(3));
        }
    }
}