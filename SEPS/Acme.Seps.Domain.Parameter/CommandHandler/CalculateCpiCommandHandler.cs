using System;
using System.Collections.Generic;
using System.Linq;
using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.Entity;
using Humanizer;

namespace Acme.Seps.Domain.Parameter.CommandHandler
{
    public sealed class CalculateCpiCommandHandler : BaseCommandHandler, ISepsCommandHandler<CalculateCpiCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;
        private readonly IRepository _repository;

        private DateTimeOffset _currentTime;

        public CalculateCpiCommandHandler(
            IRepository repository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));

            _currentTime = SystemTime.CurrentDateTime();
        }

        void ICommandHandler<CalculateCpiCommand>.Handle(CalculateCpiCommand command)
        {
            var newCpi = CreateNewCpi(command);
            _unitOfWork.Insert(newCpi);
            LogNewNaturalSellingPriceCreation(newCpi);

            GetActiveRes().ToList().ForEach(art =>
            {
                var newRenewableEnergyTariff = CreateNewRenewableEnergySourceTariff(art, newCpi);
                _unitOfWork.Insert(newRenewableEnergyTariff);
                LogNewRenewableEnergySourceTariffCreation(newRenewableEnergyTariff);
            });

            _unitOfWork.Commit();
            LogSuccessfulCommit();
        }

        private ConsumerPriceIndex CreateNewCpi(CalculateCpiCommand command) =>
            GetActiveCpi().CreateNew(command.Amount, command.Remark, _identityFactory);

        private ConsumerPriceIndex GetActiveCpi() =>
            _repository.GetSingle(new ActiveAtDateSpecification<ConsumerPriceIndex>(_currentTime));

        private IReadOnlyList<RenewableEnergySourceTariff> GetActiveRes() =>
            _repository.GetAll(new ActiveAtDateSpecification<RenewableEnergySourceTariff>(_currentTime));

        private void LogNewNaturalSellingPriceCreation(ConsumerPriceIndex cpi) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.InsertParameterLog,
                    nameof(ConsumerPriceIndex).Humanize(LetterCasing.LowerCase),
                    cpi.Period,
                    cpi.Amount)
            });

        private RenewableEnergySourceTariff CreateNewRenewableEnergySourceTariff(
            RenewableEnergySourceTariff resTariff, ConsumerPriceIndex cpi) =>
            resTariff.CreateNewWith(cpi, _identityFactory);

        private void LogNewRenewableEnergySourceTariffCreation(RenewableEnergySourceTariff resTariff) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.InsertTariffLog,
                    nameof(RenewableEnergySourceTariff).Humanize(LetterCasing.LowerCase),
                    resTariff.Period,
                    resTariff.LowerRate,
                    resTariff.HigherRate)
            });
    }
}