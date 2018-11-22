using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.Entity;
using Acme.Seps.Domain.Parameter.Repository;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.CommandHandler
{
    public sealed class CorrectActiveCpiCommandHandler
        : BaseCommandHandler, ISepsCommandHandler<CorrectActiveCpiCommand>
    {
        private readonly ISepsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CorrectActiveCpiCommandHandler(
            ISepsRepository repository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        void ICommandHandler<CorrectActiveCpiCommand>.Handle(CorrectActiveCpiCommand command)
        {
            var correctedCpi = CorrectActiveCpi();
            CorrectRenewableEnergySourceTariffs();

            ConsumerPriceIndex CorrectActiveCpi()
            {
                var activeCpi = GetActiveCpi();
                activeCpi.AmountCorrection(command.Amount, command.Remark);
                //_unitOfWork.Update(activeCpi);
                LogCpiCorrection(activeCpi);

                return activeCpi;
            }

            void CorrectRenewableEnergySourceTariffs() =>
                GetActiveRenewableEnergySourceTariffsFor(correctedCpi).ToList().ForEach(res =>
                {
                    // get by RES Type - connect those three tables
                    //res.CpiCorrection(correctedCpi, );
                    _unitOfWork.Update(res);
                    LogResCorrection(res);
                });
        }

        private ConsumerPriceIndex GetActiveCpi() =>
            _repository.GetLatest<ConsumerPriceIndex>();

        private void LogCpiCorrection(ConsumerPriceIndex cpi) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.ParameterCorrectionLog,
                    nameof(ConsumerPriceIndex).Humanize(LetterCasing.LowerCase),
                    cpi.Period,
                    cpi.Amount)
            });

        private IReadOnlyList<RenewableEnergySourceTariff> GetActiveRenewableEnergySourceTariffsFor(
            ConsumerPriceIndex cpi) =>
            _repository.GetAll(new CpiRenewableEnergySourceTariffSpecification(cpi.Id));

        private void LogResCorrection(RenewableEnergySourceTariff resTariff) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.TariffCorrectionLog,
                    nameof(RenewableEnergySourceTariff).Humanize(LetterCasing.LowerCase),
                    resTariff.Period,
                    resTariff.LowerRate,
                    resTariff.HigherRate)
            });
    }
}