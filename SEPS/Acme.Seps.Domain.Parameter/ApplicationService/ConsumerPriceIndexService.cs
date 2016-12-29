using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ApplicationService;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.DataTransferObject;
using Acme.Seps.Domain.Parameter.Entity;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.ApplicationService
{
    public sealed class ConsumerPriceIndexService
        : SepsBaseService, IEconometricIndexService<ConsumerPriceIndex, YearlyEconometricIndexDto>
    {
        private readonly IRepository<ConsumerPriceIndex> _cpiRepository;
        private readonly IRepository<RenewableEnergySourceTariff> _resRepository;

        //private readonly ITariffRepository _tariffRepository;
        //private readonly IEconometricIndexRepository _consumerPriceIndexRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IIdentityFactory<Guid> _identityFactory;

        public ConsumerPriceIndexService(
            IRepository<ConsumerPriceIndex> cpiRepository,
            IRepository<RenewableEnergySourceTariff> resRepository,
            //ITariffRepository tariffRepository,
            //IEconometricIndexRepository consumerPriceIndexRepository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
        {
            if (cpiRepository == null)
                throw new ArgumentNullException(nameof(cpiRepository));
            if (resRepository == null)
                throw new ArgumentNullException(nameof(resRepository));
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (identityFactory == null)
                throw new ArgumentNullException(nameof(identityFactory));

            _cpiRepository = cpiRepository;
            _resRepository = resRepository;
            _unitOfWork = unitOfWork;
            _identityFactory = identityFactory;
        }

        void IEconometricIndexService<ConsumerPriceIndex, YearlyEconometricIndexDto>.CalculateNewEntry(
            YearlyEconometricIndexDto econometricIndexDto)
        {
            var newCpi = GetNewCpi(econometricIndexDto);

            _unitOfWork.Insert(newCpi);

            LogNewCpi(newCpi);

            GetActiveRes().ToList().ForEach(art =>
            {
                _unitOfWork.Insert(art.CreateNewWith(newCpi, _identityFactory));

                LogNewResTariff(art);
            });

            _unitOfWork.Commit();

            LogSuccessfulCommit();
        }

        void IEconometricIndexService<ConsumerPriceIndex, YearlyEconometricIndexDto>.UpdateLastEntry(
            YearlyEconometricIndexDto econometricIndexDto)
        {
            var activeCpi = GetActiveCpi();
            var newCpi = activeCpi.CreateNew(
                econometricIndexDto.Amount, econometricIndexDto.Remark, _identityFactory) as ConsumerPriceIndex;

            _unitOfWork.Insert(newCpi);
            _unitOfWork.Delete(activeCpi);

            LogCpiUpdate(newCpi);

            GetActiveRes().ToList().ForEach(art =>
            {
                var newRes = art.CreateNewWith(newCpi, _identityFactory);

                _unitOfWork.Insert(newRes);
                _unitOfWork.Delete(art);

                LogResTariffUpdate(newRes);
            });

            _unitOfWork.Commit();

            LogSuccessfulCommit();
        }

        private ConsumerPriceIndex GetNewCpi(YearlyEconometricIndexDto econometricIndexDto) =>
            GetActiveCpi().CreateNew(
                econometricIndexDto.Amount, econometricIndexDto.Remark, _identityFactory) as ConsumerPriceIndex;

        private ConsumerPriceIndex GetActiveCpi() =>
            _cpiRepository.Get(new ActiveSpecification<ConsumerPriceIndex>()).SingleOrDefault(); //_consumerPriceIndexRepository.GetActive<ConsumerPriceIndex>();

        private IEnumerable<RenewableEnergySourceTariff> GetActiveRes() =>
            _resRepository.Get(new ActiveSpecification<RenewableEnergySourceTariff>()); //_tariffRepository.GetActive<RenewableEnergySourceTariff>();

        private void LogNewCpi(ConsumerPriceIndex cpi) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.InsertParameterLog,
                    nameof(ConsumerPriceIndex).Humanize(LetterCasing.LowerCase),
                    cpi.Period,
                    cpi.Amount)
            });

        private void LogCpiUpdate(ConsumerPriceIndex cpi) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.UpdateParameterLog,
                    nameof(ConsumerPriceIndex).Humanize(LetterCasing.LowerCase),
                    cpi.Period,
                    cpi.Amount)
            });

        private void LogNewResTariff(RenewableEnergySourceTariff res) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.InsertTariffLog,
                    nameof(RenewableEnergySourceTariff).Humanize(LetterCasing.LowerCase),
                    res.Period,
                    res.LowerRate,
                    res.HigherRate)
            });

        private void LogResTariffUpdate(RenewableEnergySourceTariff res) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.UpdateTariffLog,
                    nameof(RenewableEnergySourceTariff).Humanize(LetterCasing.LowerCase),
                    res.Period,
                    res.LowerRate,
                    res.HigherRate)
            });
    }
}