using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ApplicationService;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.DataTransferObject;
using Acme.Seps.Domain.Parameter.DomainService;
using Acme.Seps.Domain.Parameter.Entity;
using Acme.Seps.Domain.Parameter.Repository;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.ApplicationService
{
    public sealed class NaturalGasSellingPriceService :
        SepsBaseService,
        IEconometricIndexService<NaturalGasSellingPrice, MonthlyEconometricIndexDto>
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly IRepository<CogenerationTariff> _cogenerationTariffRepository;
        private readonly IRepository<NaturalGasSellingPrice> _naturalGasRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IIdentityFactory<Guid> _identityFactory;

        public NaturalGasSellingPriceService(
            ICogenerationParameterService cogenerationParameterService,
            IRepository<CogenerationTariff> cogenerationTariffRepository,
            IRepository<NaturalGasSellingPrice> naturalGasRepository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
        {
            _cogenerationParameterService =
                cogenerationParameterService ?? throw new ArgumentNullException(nameof(cogenerationParameterService));
            _cogenerationTariffRepository =
                cogenerationTariffRepository ?? throw new ArgumentNullException(nameof(cogenerationTariffRepository));
            _naturalGasRepository =
                naturalGasRepository ?? throw new ArgumentNullException(nameof(naturalGasRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        void IEconometricIndexService<NaturalGasSellingPrice, MonthlyEconometricIndexDto>.CalculateNewEntry(
            MonthlyEconometricIndexDto econometricIndexDto)
        {
            var newNaturalGasSellingPrice = GetNewNaturalGasSellingPrice(econometricIndexDto);
            _unitOfWork.Insert(newNaturalGasSellingPrice);

            LogNewNaturalGasSellingPrice(newNaturalGasSellingPrice);

            var yearsNaturalGasSellingPrices = GetNaturalGasPricesWithinYear(econometricIndexDto.Year);

            GetActiveCogenerations().ToList()
                .ForEach(act =>
                {
                    _unitOfWork.Insert(act.CreateNewWith(
                         yearsNaturalGasSellingPrices,
                         _cogenerationParameterService,
                         newNaturalGasSellingPrice,
                         _identityFactory));

                    LogNewCogenerationTariff(act);
                });

            _unitOfWork.Commit();

            LogSuccessfulCommit();
        }

        void IEconometricIndexService<NaturalGasSellingPrice, MonthlyEconometricIndexDto>.UpdateLastEntry(
            MonthlyEconometricIndexDto econometricIndexDto)
        {
            var activeNaturalGasSellingPrice = GetActiveNaturalGasSellingPrice();
            var newNaturalGasSellingPrice = activeNaturalGasSellingPrice.CreateNew(
                econometricIndexDto.Amount,
                econometricIndexDto.Remark,
                econometricIndexDto.Month,
                econometricIndexDto.Year,
                _identityFactory) as NaturalGasSellingPrice;

            _unitOfWork.Insert(newNaturalGasSellingPrice);
            _unitOfWork.Delete(activeNaturalGasSellingPrice);

            LogNaturalSellingPriceUpdate(newNaturalGasSellingPrice);

            var yearsNaturalGasSellingPrices = GetNaturalGasPricesWithinYear(econometricIndexDto.Year);

            GetActiveCogenerations().ToList().ForEach(act =>
            {
                _unitOfWork.Insert(act.CreateNewWith(
                    yearsNaturalGasSellingPrices,
                    _cogenerationParameterService,
                    newNaturalGasSellingPrice,
                    _identityFactory));
                _unitOfWork.Delete(act);

                LogCogenerationTariffUpdate(act);
            });

            _unitOfWork.Commit();

            LogSuccessfulCommit();
        }

        private NaturalGasSellingPrice GetNewNaturalGasSellingPrice(MonthlyEconometricIndexDto econometricIndexDto) =>
            GetActiveNaturalGasSellingPrice().CreateNew(
                econometricIndexDto.Amount,
                econometricIndexDto.Remark,
                econometricIndexDto.Month,
                econometricIndexDto.Year,
                _identityFactory) as NaturalGasSellingPrice;

        private NaturalGasSellingPrice GetActiveNaturalGasSellingPrice() =>
            _naturalGasRepository.Get(new ActiveSpecification<NaturalGasSellingPrice>()).SingleOrDefault();//_econometricIndexRepository.GetActive<NaturalGasSellingPrice>();

        private IEnumerable<NaturalGasSellingPrice> GetNaturalGasPricesWithinYear(int year) =>
            _naturalGasRepository.Get(new YearsNaturalGasSellingPricesSpecification(year));//_naturalGasSellingPriceRepository.GetAllWithin(year);

        private IEnumerable<CogenerationTariff> GetActiveCogenerations() =>
            _cogenerationTariffRepository.Get(new ActiveSpecification<CogenerationTariff>());//_tariffRepository.GetActive<CogenerationTariff>();

        private void LogNewNaturalGasSellingPrice(NaturalGasSellingPrice naturalGasSellingPrice) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.InsertParameterLog,
                    nameof(NaturalGasSellingPrice).Humanize(LetterCasing.LowerCase),
                    naturalGasSellingPrice.Period,
                    naturalGasSellingPrice.Amount)
            });

        private void LogNaturalSellingPriceUpdate(NaturalGasSellingPrice naturalGasSellingPrice) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.UpdateParameterLog,
                    nameof(NaturalGasSellingPrice).Humanize(LetterCasing.LowerCase),
                    naturalGasSellingPrice.Period,
                    naturalGasSellingPrice.Amount)
            });

        private void LogNewCogenerationTariff(CogenerationTariff cogeneration) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.InsertTariffLog,
                    nameof(CogenerationTariff).Humanize(LetterCasing.LowerCase),
                    cogeneration.Period,
                    cogeneration.LowerRate,
                    cogeneration.HigherRate)
            });

        private void LogCogenerationTariffUpdate(CogenerationTariff cogeneration) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.UpdateTariffLog,
                    nameof(CogenerationTariff).Humanize(LetterCasing.LowerCase),
                    cogeneration.Period,
                    cogeneration.LowerRate,
                    cogeneration.HigherRate)
            });
    }
}