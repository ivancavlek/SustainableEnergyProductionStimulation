using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.Entity;
using Humanizer;
using System;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.CommandHandler
{
    public sealed class CorrectActiveCpiCommandHandler
        : BaseCommandHandler, ISepsCommandHandler<CorrectActiveCpiCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CorrectActiveCpiCommandHandler(
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        void ICommandHandler<CorrectActiveCpiCommand>.Handle(CorrectActiveCpiCommand command)
        {
            var newCpi = CreateNewCpi(command);
            _unitOfWork.Insert(newCpi);
            _unitOfWork.Delete(newCpi);
            LogCpiUpdate(newCpi);

            command.ActiveResTariffs.ToList().ForEach(art =>
            {
                var newRenewableEnergyTariff = CreateNewRenewableEnergySourceTariff(art, newCpi);
                _unitOfWork.Insert(newRenewableEnergyTariff);
                _unitOfWork.Delete(newRenewableEnergyTariff);
                LogNewRenewableEnergySourceTariffCreation(newRenewableEnergyTariff);
            });

            _unitOfWork.Commit();
            LogSuccessfulCommit();
        }

        private ConsumerPriceIndex CreateNewCpi(CorrectActiveCpiCommand command) =>
            command.ActiveCpi.CreateNew(command.Amount, command.Remark, _identityFactory);

        private void LogCpiUpdate(ConsumerPriceIndex cpi) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.UpdateParameterLog,
                    nameof(ConsumerPriceIndex).Humanize(LetterCasing.LowerCase),
                    cpi.YearlyPeriod,
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
                    resTariff.YearlyPeriod,
                    resTariff.LowerRate,
                    resTariff.HigherRate)
            });
    }
}