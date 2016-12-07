using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ApplicationService;
using Acme.Seps.Domain.Parameter.DataTransferObject;
using Acme.Seps.Domain.Parameter.Entity;
using Acme.Seps.Domain.Parameter.Repository;
using Humanizer;
using System;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.ApplicationService
{
    public sealed class ConsumerPriceIndexService
        : SepsBaseService, IEconometricIndexService<ConsumerPriceIndex, YearlyEconometricIndexDto>
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

            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.InsertParameterLog,
                    nameof(ConsumerPriceIndex).Humanize(LetterCasing.LowerCase),
                    newCpi.Period,
                    newCpi.Amount)
            });

            activeResTariffs.ToList().ForEach(art =>
            {
                _unitOfWork.Insert(art.CreateNewWith(newCpi, _identityFactory));

                Log(new EntityExecutionLoggingEventArgs
                {
                    Message = string.Format(
                    Infrastructure.Parameter.InsertTariffLog,
                    nameof(RenewableEnergySourceTariff).Humanize(LetterCasing.LowerCase),
                    art.Period,
                    art.LowerRate,
                    art.HigherRate)
                });
            });

            _unitOfWork.Insert(newCpi);
            _unitOfWork.Commit();

            Log(new EntityExecutionLoggingEventArgs { Message = Infrastructure.Parameter.SuccessfulSave });
        }

        void IEconometricIndexService<ConsumerPriceIndex, YearlyEconometricIndexDto>.UpdateLastEntry(
            YearlyEconometricIndexDto econometricIndexDto)
        {
            var activeCpi = _consumerPriceIndexRepository.GetActive<ConsumerPriceIndex>();
            var activeResTariffs = _tariffRepository.GetActive<RenewableEnergySourceTariff>();

            var newCpi = activeCpi.CreateNew(
                econometricIndexDto.Amount, econometricIndexDto.Remark, _identityFactory) as ConsumerPriceIndex;

            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.UpdateParameterLog,
                    nameof(ConsumerPriceIndex).Humanize(LetterCasing.LowerCase),
                    newCpi.Period,
                    newCpi.Amount)
            });

            activeResTariffs.ToList().ForEach(art =>
            {
                _unitOfWork.Insert(art.CreateNewWith(newCpi, _identityFactory));
                _unitOfWork.Delete(art);

                Log(new EntityExecutionLoggingEventArgs
                {
                    Message = string.Format(
                    Infrastructure.Parameter.UpdateTariffLog,
                    nameof(RenewableEnergySourceTariff).Humanize(LetterCasing.LowerCase),
                    art.Period,
                    art.LowerRate,
                    art.HigherRate)
                });
            });

            _unitOfWork.Insert(newCpi);
            _unitOfWork.Delete(activeCpi);
            _unitOfWork.Commit();

            Log(new EntityExecutionLoggingEventArgs { Message = Infrastructure.Parameter.SuccessfulSave });
        }
    }
}