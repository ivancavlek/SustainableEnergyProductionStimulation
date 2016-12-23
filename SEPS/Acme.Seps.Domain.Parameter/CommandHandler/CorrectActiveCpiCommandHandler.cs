using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ApplicationService;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.Entity;
using Humanizer;
using System;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.CommandHandler
{
    public sealed class CorrectActiveCpiCommandHandler
        : BaseCommandHandler, ICommandHandler<CorrectActiveCpiCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CorrectActiveCpiCommandHandler(
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory,
            ISepsLogService sepsLogService) : base(sepsLogService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (identityFactory == null)
                throw new ArgumentNullException(nameof(identityFactory));

            _unitOfWork = unitOfWork;
            _identityFactory = identityFactory;
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
            command.ActiveCpi.CreateNew(
                command.Amount, command.Remark, _identityFactory) as ConsumerPriceIndex;

        private void LogCpiUpdate(ConsumerPriceIndex cpi) =>
            SepsLogService.Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.UpdateParameterLog,
                    nameof(ConsumerPriceIndex).Humanize(LetterCasing.LowerCase),
                    cpi.Period,
                    cpi.Amount)
            });

        private RenewableEnergySourceTariff CreateNewRenewableEnergySourceTariff(
            RenewableEnergySourceTariff resTariff, ConsumerPriceIndex cpi) =>
            resTariff.CreateNewWith(cpi, _identityFactory);

        private void LogNewRenewableEnergySourceTariffCreation(RenewableEnergySourceTariff resTariff) =>
            SepsLogService.Log(new EntityExecutionLoggingEventArgs
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