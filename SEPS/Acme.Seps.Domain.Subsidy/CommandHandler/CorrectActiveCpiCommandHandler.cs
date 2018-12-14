﻿using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Subsidy.Command;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using Acme.Seps.Domain.Subsidy.Repository;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Subsidy.CommandHandler
{
    public sealed class CorrectActiveCpiCommandHandler
        : BaseCommandHandler, ISepsCommandHandler<CorrectActiveCpiCommand>
    {
        private readonly IRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CorrectActiveCpiCommandHandler(
            IRepository repository, IUnitOfWork unitOfWork, IIdentityFactory<Guid> identityFactory)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        void ICommandHandler<CorrectActiveCpiCommand>.Handle(CorrectActiveCpiCommand command)
        {
            var correctedCpi = CorrectActiveCpi();
            CorrectRenewableEnergySourceTariffs();

            _unitOfWork.Commit();
            LogSuccessfulCommit();

            ConsumerPriceIndex CorrectActiveCpi()
            {
                var activeCpi = GetActiveConsumerPriceIndex();
                activeCpi.Correct(command.Amount, command.Remark);
                LogConsumerPriceIndexCorrection(activeCpi);

                return activeCpi;
            }

            void CorrectRenewableEnergySourceTariffs()
            {
                var previousRes = GetPreviousActiveRenewableEnergySourceTariffsFor(correctedCpi.Period.ActiveFrom);

                GetActiveRenewableEnergySourceTariffsFor().ToList().ForEach(res =>
                {
                    res.CpiCorrection(
                        correctedCpi, PreviousRenewableEnergySourceBy(res.ProjectTypeId, res.LowerProductionLimit));
                    _unitOfWork.Update(res);
                    LogRenewableEnergySourceTariffCorrection(res);
                });

                RenewableEnergySourceTariff PreviousRenewableEnergySourceBy(
                    Guid projectTypeId, decimal? lowerProductionLimit) =>
                    previousRes.Single(res =>
                        res.ProjectTypeId.Equals(projectTypeId) && res.LowerProductionLimit.Equals(lowerProductionLimit));
            }
        }

        private ConsumerPriceIndex GetActiveConsumerPriceIndex() =>
            _repository.GetSingle(new ActiveSpecification<ConsumerPriceIndex>());

        private void LogConsumerPriceIndexCorrection(ConsumerPriceIndex cpi) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    SubsidyMessages.ParameterCorrectionLog,
                    nameof(ConsumerPriceIndex).Humanize(LetterCasing.LowerCase),
                    cpi.Period,
                    cpi.Amount)
            });

        private IReadOnlyList<RenewableEnergySourceTariff> GetPreviousActiveRenewableEnergySourceTariffsFor(
            DateTimeOffset activeTill) =>
            _repository.GetAll(new PreviousActiveSpecification<RenewableEnergySourceTariff>(activeTill));

        private IReadOnlyList<RenewableEnergySourceTariff> GetActiveRenewableEnergySourceTariffsFor() =>
            _repository.GetAll(new ActiveSpecification<RenewableEnergySourceTariff>());

        private void LogRenewableEnergySourceTariffCorrection(RenewableEnergySourceTariff resTariff) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    SubsidyMessages.TariffCorrectionLog,
                    nameof(RenewableEnergySourceTariff).Humanize(LetterCasing.LowerCase),
                    resTariff.Period,
                    resTariff.LowerRate,
                    resTariff.HigherRate)
            });
    }
}