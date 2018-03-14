using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.ApplicationService;
using Acme.Seps.Domain.Parameter.DataTransferObject;
using Acme.Seps.Domain.Parameter.Entity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.ApplicationService
{
    public class ConsumerPriceIndexServiceTests
    {
        private readonly IEconometricIndexService<ConsumerPriceIndex, YearlyEconometricIndexDto> _cpiService;
        private readonly IRepository<RenewableEnergySourceTariff> _resRepository;
        private readonly IRepository<ConsumerPriceIndex> _cpiRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConsumerPriceIndexServiceTests()
        {
            var _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
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
            _resRepository = Substitute.For<IRepository<RenewableEnergySourceTariff>>();
            _resRepository
                .Get(Arg.Any<ISpecification<RenewableEnergySourceTariff>>())
                .Returns(new List<RenewableEnergySourceTariff> { renewableEnergySourceTariff });
            _cpiRepository = Substitute.For<IRepository<ConsumerPriceIndex>>();
            _cpiRepository
                .Get(Arg.Any<ISpecification<ConsumerPriceIndex>>())
                .Returns(new List<ConsumerPriceIndex> { _cpi });
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _cpiService = new ConsumerPriceIndexService(
                _cpiRepository, _resRepository, _unitOfWork, _identityFactory);
        }

        public void NewCpiIsSuccessfullyCreated()
        {
            var yaep = new YearlyEconometricIndexDto { Amount = 100M, Remark = nameof(YearlyEconometricIndexDto) };

            _cpiService.CalculateNewEntry(yaep);

            _resRepository.Received().Get(Arg.Any<ISpecification<RenewableEnergySourceTariff>>());
            _cpiRepository.Received().Get(Arg.Any<ISpecification<ConsumerPriceIndex>>());
            _unitOfWork.Received().Insert(Arg.Any<RenewableEnergySourceTariff>());
            _unitOfWork.Received().Insert(Arg.Any<ConsumerPriceIndex>());
            _unitOfWork.Received().Commit();
        }

        public void LastCpiEntryIsSuccessfullyUpdated()
        {
            var yaep = new YearlyEconometricIndexDto { Amount = 100M, Remark = nameof(YearlyEconometricIndexDto) };

            _cpiService.UpdateLastEntry(yaep);

            _resRepository.Received().Get(Arg.Any<ISpecification<RenewableEnergySourceTariff>>());
            _cpiRepository.Received().Get(Arg.Any<ISpecification<ConsumerPriceIndex>>());
            _unitOfWork.Received().Delete(Arg.Any<RenewableEnergySourceTariff>());
            _unitOfWork.Received().Insert(Arg.Any<RenewableEnergySourceTariff>());
            _unitOfWork.Received().Delete(Arg.Any<ConsumerPriceIndex>());
            _unitOfWork.Received().Insert(Arg.Any<ConsumerPriceIndex>());
            _unitOfWork.Received().Commit();
        }
    }
}