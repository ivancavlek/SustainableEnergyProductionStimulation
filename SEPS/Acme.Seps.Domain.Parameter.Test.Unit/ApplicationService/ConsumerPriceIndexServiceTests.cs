using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.ApplicationService;
using Acme.Seps.Domain.Parameter.DataTransferObject;
using Acme.Seps.Domain.Parameter.Entity;
using Acme.Seps.Domain.Parameter.Repository;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.ApplicationService
{
    public class ConsumerPriceIndexServiceTests
    {
        private readonly IEconometricIndexService<ConsumerPriceIndex, YearlyEconometricIndexDto> _cpiService;

        private readonly Mock<ITariffRepository> _tariffRepository;
        private readonly Mock<IEconometricIndexRepository> _econometricIndexRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;

        private readonly ConsumerPriceIndex _cpi;

        public ConsumerPriceIndexServiceTests()
        {
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

            _tariffRepository = new Mock<ITariffRepository>();
            _tariffRepository
                .Setup(m => m.GetActive<RenewableEnergySourceTariff>())
                .Returns(new List<RenewableEnergySourceTariff> { Activator.CreateInstance(
                    typeof(RenewableEnergySourceTariff),
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { _cpi, 5M, 10M, _identityFactory.Object },
                    null) as RenewableEnergySourceTariff });
            _econometricIndexRepository = new Mock<IEconometricIndexRepository>();
            _econometricIndexRepository
                .Setup(m => m.GetActive<ConsumerPriceIndex>())
                .Returns(_cpi);
            _unitOfWork = new Mock<IUnitOfWork>();

            _cpiService = new ConsumerPriceIndexService(
                _tariffRepository.Object, _econometricIndexRepository.Object, _unitOfWork.Object, _identityFactory.Object);
        }

        public void NewCpiIsSuccessfullyCreated()
        {
            var yaep = new YearlyEconometricIndexDto { Amount = 100M, Remark = nameof(YearlyEconometricIndexDto) };

            _cpiService.CalculateNewEntry(yaep);

            _tariffRepository.Verify(m => m.GetActive<RenewableEnergySourceTariff>(), Times.Once);
            _econometricIndexRepository.Verify(m => m.GetActive<ConsumerPriceIndex>(), Times.Once);
            _unitOfWork.Verify(m => m.Insert(It.IsAny<RenewableEnergySourceTariff>()), Times.Once);
            _unitOfWork.Verify(m => m.Insert(It.IsAny<ConsumerPriceIndex>()), Times.Once);
            _unitOfWork.Verify(m => m.Commit(), Times.Once);
        }

        public void LastCpiEntryIsSuccessfullyUpdated()
        {
            var yaep = new YearlyEconometricIndexDto { Amount = 100M, Remark = nameof(YearlyEconometricIndexDto) };

            _cpiService.UpdateLastEntry(yaep);

            _tariffRepository.Verify(m => m.GetActive<RenewableEnergySourceTariff>(), Times.Once);
            _econometricIndexRepository.Verify(m => m.GetActive<ConsumerPriceIndex>(), Times.Once);
            _unitOfWork.Verify(m => m.Delete(It.IsAny<RenewableEnergySourceTariff>()), Times.Once);
            _unitOfWork.Verify(m => m.Insert(It.IsAny<RenewableEnergySourceTariff>()), Times.Once);
            _unitOfWork.Verify(m => m.Delete(It.IsAny<ConsumerPriceIndex>()), Times.Once);
            _unitOfWork.Verify(m => m.Insert(It.IsAny<ConsumerPriceIndex>()), Times.Once);
            _unitOfWork.Verify(m => m.Commit(), Times.Once);
        }
    }
}