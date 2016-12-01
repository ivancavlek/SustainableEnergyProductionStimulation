using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.DataTransferObject;
using Acme.Seps.Domain.Parameter.Entity;
using Acme.Seps.Domain.Parameter.Repository;
using System;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.ApplicationService
{
    public class ConsumerPriceIndexService
        : IEconometricIndexService<ConsumerPriceIndex, YearlyEconometricIndexDto>
    {
        private readonly ITariffRepository _tariffRepository;
        private readonly IEconometricIndexRepository _consumerPriceIndexRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public ConsumerPriceIndexService(
            ITariffRepository tariffRepository,
            IEconometricIndexRepository consumerPriceIndexRepository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
        {
            if (tariffRepository == null)
                throw new ArgumentNullException(nameof(tariffRepository));
            if (consumerPriceIndexRepository == null)
                throw new ArgumentNullException(nameof(consumerPriceIndexRepository));
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (identityFactory == null)
                throw new ArgumentNullException(nameof(identityFactory));

            _tariffRepository = tariffRepository;
            _consumerPriceIndexRepository = consumerPriceIndexRepository;
            _unitOfWork = unitOfWork;
            _identityFactory = identityFactory;
        }

        void IEconometricIndexService<ConsumerPriceIndex, YearlyEconometricIndexDto>.CalculateNewEntry(
            YearlyEconometricIndexDto econometricIndexDto)
        {
            var activeCpi = _consumerPriceIndexRepository.GetActive<ConsumerPriceIndex>();
            var activeResTariffs = _tariffRepository.GetActive<RenewableEnergySourceTariff>();

            var newCpi = activeCpi.CreateNew(
                econometricIndexDto.Amount, econometricIndexDto.Remark, _identityFactory) as ConsumerPriceIndex;
            activeResTariffs.ToList().ForEach(art => _unitOfWork.Insert(art.CreateNewWith(newCpi, _identityFactory)));

            _unitOfWork.Insert(newCpi);
            _unitOfWork.Commit();
        }

        void IEconometricIndexService<ConsumerPriceIndex, YearlyEconometricIndexDto>.UpdateLastEntry(
            YearlyEconometricIndexDto econometricIndexDto)
        {
            var activeCpi = _consumerPriceIndexRepository.GetActive<ConsumerPriceIndex>();
            var activeResTariffs = _tariffRepository.GetActive<RenewableEnergySourceTariff>();

            var newCpi = activeCpi.CreateNew(
                econometricIndexDto.Amount, econometricIndexDto.Remark, _identityFactory) as ConsumerPriceIndex;
            activeResTariffs.ToList().ForEach(art =>
            {
                _unitOfWork.Insert(art.CreateNewWith(newCpi, _identityFactory));
                _unitOfWork.Delete(art);
            });

            _unitOfWork.Insert(newCpi);
            _unitOfWork.Delete(activeCpi);
            _unitOfWork.Commit();
        }
    }
}