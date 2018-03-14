using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.ApplicationService;
using Acme.Seps.Domain.Parameter.DataTransferObject;
using Acme.Seps.Domain.Parameter.DomainService;
using Acme.Seps.Domain.Parameter.Entity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.ApplicationService
{
    public class NaturalGasSellingPriceServiceTests
    {
        private readonly IEconometricIndexService<NaturalGasSellingPrice, MonthlyEconometricIndexDto> _naturalGasService;

        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly IRepository<NaturalGasSellingPrice> _naturalGasSellingPriceRepository;
        private readonly IRepository<CogenerationTariff> _tariffRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly DateTime _lastPeriod;

        public NaturalGasSellingPriceServiceTests()
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
                new object[] {
                    10M,
                    nameof(NaturalGasSellingPrice),
                    new MonthlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                    _identityFactory },
                null) as NaturalGasSellingPrice;

            _naturalGasSellingPriceRepository = Substitute.For<IRepository<NaturalGasSellingPrice>>();
            _naturalGasSellingPriceRepository
                .Get(Arg.Any<ISpecification<NaturalGasSellingPrice>>())
                .Returns(new List<NaturalGasSellingPrice> { _naturalGasSellingPrice });

            _tariffRepository = Substitute.For<IRepository<CogenerationTariff>>();
            _tariffRepository
                .Get(Arg.Any<ISpecification<CogenerationTariff>>())
                .Returns(new List<CogenerationTariff> { Activator.CreateInstance(
                    typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    _naturalGasSellingPrice,
                    10M,
                    10M,
                    new MonthlyPeriod(DateTime.Now.AddMonths(-4), _lastPeriod),
                    _identityFactory },
                null) as CogenerationTariff });
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _naturalGasService = new NaturalGasSellingPriceService(
                _cogenerationParameterService,
                _tariffRepository,
                _naturalGasSellingPriceRepository,
                _unitOfWork,
                _identityFactory);
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

            _unitOfWork.Received().Insert(Arg.Any<NaturalGasSellingPrice>());
            _naturalGasSellingPriceRepository.Received(2).Get(Arg.Any<ISpecification<NaturalGasSellingPrice>>());
            _tariffRepository.Received().Get(Arg.Any<ISpecification<CogenerationTariff>>());
            _unitOfWork.Received().Insert(Arg.Any<CogenerationTariff>());
            _unitOfWork.Received().Commit();
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

            _unitOfWork.Received().Insert(Arg.Any<NaturalGasSellingPrice>());
            _unitOfWork.Received().Delete(Arg.Any<NaturalGasSellingPrice>());
            _naturalGasSellingPriceRepository.Received(2).Get(Arg.Any<ISpecification<NaturalGasSellingPrice>>());
            _tariffRepository.Received().Get(Arg.Any<ISpecification<CogenerationTariff>>());
            _unitOfWork.Received().Insert(Arg.Any<CogenerationTariff>());
            _unitOfWork.Received().Delete(Arg.Any<CogenerationTariff>());
            _unitOfWork.Received().Commit();
        }
    }
}