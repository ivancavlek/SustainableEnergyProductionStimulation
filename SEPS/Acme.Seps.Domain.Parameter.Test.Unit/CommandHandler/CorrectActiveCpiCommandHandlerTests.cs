using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ApplicationService;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.CommandHandler;
using Acme.Seps.Domain.Parameter.Entity;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.CommandHandler
{
    public class CorrectActiveCpiCommandHandlerTests
    {
        private readonly ICommandHandler<CorrectActiveCpiCommand> _correctActiveCpi;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;
        private readonly Mock<ISepsLogService> _sepsLogService;

        private readonly ConsumerPriceIndex _cpi;
        private readonly IEnumerable<RenewableEnergySourceTariff> _resTariff;

        public CorrectActiveCpiCommandHandlerTests()
        {
            _sepsLogService = new Mock<ISepsLogService>();
            _identityFactory = new Mock<IIdentityFactory<Guid>>();
            _identityFactory.Setup(m => m.CreateIdentity()).Returns(Guid.NewGuid());

            _cpi = Activator.CreateInstance(
                typeof(ConsumerPriceIndex),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    10M,
                    nameof(ConsumerPriceIndex),
                    new YearlyPeriod(DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-3)),
                    _identityFactory.Object },
                null) as ConsumerPriceIndex;

            _resTariff = new List<RenewableEnergySourceTariff> { Activator.CreateInstance(
                typeof(RenewableEnergySourceTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { _cpi, 5M, 10M, _identityFactory.Object },
                null) as RenewableEnergySourceTariff };

            _unitOfWork = new Mock<IUnitOfWork>();

            _correctActiveCpi = new CorrectActiveCpiCommandHandler(
                _unitOfWork.Object, _identityFactory.Object, _sepsLogService.Object);
        }

        public void ExecutesProperly()
        {
            var correctActiveCpi = new CorrectActiveCpiCommand
            {
                ActiveCpi = _cpi,
                ActiveResTariffs = _resTariff,
                Amount = 100M,
                Remark = nameof(CalculateCpiCommand)
            };

            _correctActiveCpi.Handle(correctActiveCpi);

            _unitOfWork.Verify(m => m.Delete(It.IsAny<ConsumerPriceIndex>()), Times.Once);
            _unitOfWork.Verify(m => m.Insert(It.IsAny<ConsumerPriceIndex>()), Times.Once);
            _unitOfWork.Verify(m => m.Delete(It.IsAny<RenewableEnergySourceTariff>()), Times.Once);
            _unitOfWork.Verify(m => m.Insert(It.IsAny<RenewableEnergySourceTariff>()), Times.Once);
            _unitOfWork.Verify(m => m.Commit(), Times.Once);
            _sepsLogService.Verify(m => m.Log(It.IsAny<EntityExecutionLoggingEventArgs>()), Times.Exactly(3));
        }
    }
}