using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.DataTransferObject;
using Acme.Seps.Domain.Parameter.DomainService;
using Acme.Seps.Domain.Parameter.Entity;
using Acme.Seps.Domain.Parameter.Repository;
using System;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.ApplicationService
{
    public sealed class NaturalGasSellingPriceService :
        IEconometricIndexService<NaturalGasSellingPrice, MonthlyEconometricIndexDto>
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly ITariffRepository _tariffRepository;
        private readonly INaturalGasSellingPriceRepository _naturalGasSellingPriceRepository;
        private readonly IEconometricIndexRepository _econometricIndexRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public NaturalGasSellingPriceService(
            ICogenerationParameterService cogenerationParameterService,
            ITariffRepository tariffRepository,
            INaturalGasSellingPriceRepository naturalGasSellingPriceRepository,
            IEconometricIndexRepository econometricIndexRepository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
        {
            if (cogenerationParameterService == null)
                throw new ArgumentNullException(nameof(cogenerationParameterService));
            if (tariffRepository == null)
                throw new ArgumentNullException(nameof(tariffRepository));
            if (naturalGasSellingPriceRepository == null)
                throw new ArgumentNullException(nameof(naturalGasSellingPriceRepository));
            if (econometricIndexRepository == null)
                throw new ArgumentNullException(nameof(econometricIndexRepository));
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (identityFactory == null)
                throw new ArgumentNullException(nameof(identityFactory));

            _cogenerationParameterService = cogenerationParameterService;
            _tariffRepository = tariffRepository;
            _naturalGasSellingPriceRepository = naturalGasSellingPriceRepository;
            _econometricIndexRepository = econometricIndexRepository;
            _unitOfWork = unitOfWork;
            _identityFactory = identityFactory;
        }

        void IEconometricIndexService<NaturalGasSellingPrice, MonthlyEconometricIndexDto>.CalculateNewEntry(
            MonthlyEconometricIndexDto econometricIndexDto)
        {
            var activeNaturalGasSellingPrice = _econometricIndexRepository.GetActive<NaturalGasSellingPrice>();
            var activeCogenerationTariffs = _tariffRepository.GetActive<CogenerationTariff>();
            var yearsNaturalGasSellingPrices = _naturalGasSellingPriceRepository.GetAllWithin(econometricIndexDto.Year);

            var newNaturalGasSellingPrice = activeNaturalGasSellingPrice.CreateNew(
                econometricIndexDto.Amount,
                econometricIndexDto.Remark,
                econometricIndexDto.Month,
                econometricIndexDto.Year,
                _identityFactory) as NaturalGasSellingPrice;
            activeCogenerationTariffs.ToList()
                .ForEach(act => _unitOfWork.Insert(act.CreateNewWith(
                    yearsNaturalGasSellingPrices,
                    _cogenerationParameterService,
                    newNaturalGasSellingPrice,
                    _identityFactory)));

            _unitOfWork.Insert(newNaturalGasSellingPrice);
            _unitOfWork.Commit();
        }

        void IEconometricIndexService<NaturalGasSellingPrice, MonthlyEconometricIndexDto>.UpdateLastEntry(
            MonthlyEconometricIndexDto econometricIndexDto)
        {
            var activeNaturalGasSellingPrice = _econometricIndexRepository.GetActive<NaturalGasSellingPrice>();
            var activeCogenerationTariffs = _tariffRepository.GetActive<CogenerationTariff>();
            var yearsNaturalGasSellingPrices = _naturalGasSellingPriceRepository.GetAllWithin(econometricIndexDto.Year);

            var newNaturalGasSellingPrice = activeNaturalGasSellingPrice.CreateNew(
                econometricIndexDto.Amount,
                econometricIndexDto.Remark,
                econometricIndexDto.Month,
                econometricIndexDto.Year,
                _identityFactory) as NaturalGasSellingPrice;
            activeCogenerationTariffs.ToList().ForEach(act =>
            {
                _unitOfWork.Insert(act.CreateNewWith(
                    yearsNaturalGasSellingPrices,
                    _cogenerationParameterService,
                    newNaturalGasSellingPrice,
                    _identityFactory));
                _unitOfWork.Delete(act);
            });

            _unitOfWork.Insert(newNaturalGasSellingPrice);
            _unitOfWork.Delete(activeNaturalGasSellingPrice);
            _unitOfWork.Commit();
        }
    }
}