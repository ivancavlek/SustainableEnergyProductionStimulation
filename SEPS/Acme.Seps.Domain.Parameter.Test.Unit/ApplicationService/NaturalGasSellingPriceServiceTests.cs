using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.ApplicationService;
using Acme.Seps.Domain.Parameter.DataTransferObject;
using Acme.Seps.Domain.Parameter.DomainService;
using Acme.Seps.Domain.Parameter.Entity;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.ApplicationService
{
    public class NaturalGasSellingPriceServiceTests
    {
        private readonly IEconometricIndexService<NaturalGasSellingPrice, MonthlyEconometricIndexDto> _naturalGasService;

        private readonly Mock<ICogenerationParameterService> _cogenerationParameterService;
        private readonly Mock<IRepository<NaturalGasSellingPrice>> _naturalGasSellingPriceRepository;
        private readonly Mock<IRepository<CogenerationTariff>> _tariffRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;

        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly DateTime _lastPeriod;

        public NaturalGasSellingPriceServiceTests()
        {
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

            _naturalGasSellingPriceRepository = new Mock<IRepository<NaturalGasSellingPrice>>();
            _naturalGasSellingPriceRepository
                .Setup(m => m.Get(It.IsAny<ISpecification<NaturalGasSellingPrice>>()))
                .Returns(new List<NaturalGasSellingPrice> { _naturalGasSellingPrice });

            _tariffRepository = new Mock<IRepository<CogenerationTariff>>();
            _tariffRepository
                .Setup(m => m.Get(It.IsAny<ISpecification<CogenerationTariff>>()))
                .Returns(new List<CogenerationTariff> { Activator.CreateInstance(
                    typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    _naturalGasSellingPrice,
                    10M,
                    10M,
                    new MonthlyPeriod(DateTime.Now.AddMonths(-4), _lastPeriod),
                    _identityFactory.Object },
                null) as CogenerationTariff });
            _unitOfWork = new Mock<IUnitOfWork>();

            _naturalGasService = new NaturalGasSellingPriceService(
                _cogenerationParameterService.Object,
                _tariffRepository.Object,
                _naturalGasSellingPriceRepository.Object,
                _unitOfWork.Object,
                _identityFactory.Object);
        }

        public void NewNaturalGasIsSuccessfullyCreated()
        {
            var maep = new MonthlyEconometricIndexDto
            {
                Month = _lastPeriod.Month,
                Year = _lastPeriod.Year,
                Amount = 100M,
                Remark = nameof(MonthlyEconometricIndexDto)
            };

            _naturalGasService.CalculateNewEntry(maep);

            _unitOfWork.Verify(m => m.Insert(It.IsAny<NaturalGasSellingPrice>()), Times.Once);
            _naturalGasSellingPriceRepository.Verify(m => m.Get(It.IsAny<ISpecification<NaturalGasSellingPrice>>()), Times.Exactly(2));
            _tariffRepository.Verify(m => m.Get(It.IsAny<ISpecification<CogenerationTariff>>()), Times.Once);
            _unitOfWork.Verify(m => m.Insert(It.IsAny<CogenerationTariff>()), Times.Once);
            _unitOfWork.Verify(m => m.Commit(), Times.Once);
        }

        public void LastNaturalGasEntryIsSuccessfullyUpdated()
        {
            var maep = new MonthlyEconometricIndexDto
            {
                Month = _lastPeriod.Month,
                Year = _lastPeriod.Year,
                Amount = 100M,
                Remark = nameof(MonthlyEconometricIndexDto)
            };

            _naturalGasService.UpdateLastEntry(maep);

            _unitOfWork.Verify(m => m.Insert(It.IsAny<NaturalGasSellingPrice>()), Times.Once);
            _unitOfWork.Verify(m => m.Delete(It.IsAny<NaturalGasSellingPrice>()), Times.Once);
            _naturalGasSellingPriceRepository.Verify(m => m.Get(It.IsAny<ISpecification<NaturalGasSellingPrice>>()), Times.Exactly(2));
            _tariffRepository.Verify(m => m.Get(It.IsAny<ISpecification<CogenerationTariff>>()), Times.Once);
            _unitOfWork.Verify(m => m.Insert(It.IsAny<CogenerationTariff>()), Times.Once);
            _unitOfWork.Verify(m => m.Delete(It.IsAny<CogenerationTariff>()), Times.Once);
            _unitOfWork.Verify(m => m.Commit(), Times.Once);
        }
    }
}