using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Subsidy.Command;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using Acme.Seps.Domain.Subsidy.Repository;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Subsidy.CommandHandler
{
    public sealed class CorrectActiveNaturalGasCommandHandler
        : BaseCommandHandler, ISepsCommandHandler<CorrectActiveNaturalGasCommand>
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly ISepsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CorrectActiveNaturalGasCommandHandler(
            ICogenerationParameterService cogenerationParameterService,
            ISepsRepository repository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cogenerationParameterService =
                cogenerationParameterService ?? throw new ArgumentNullException(nameof(cogenerationParameterService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
            _repository = repository;
        }

        void ICommandHandler<CorrectActiveNaturalGasCommand>.Handle(
            CorrectActiveNaturalGasCommand command)
        {
            var correctedGsp = CorrectActiveGsp();
            CorrectCogenerationTariffs();

            _unitOfWork.Commit();
            LogSuccessfulCommit();

            NaturalGasSellingPrice CorrectActiveGsp()
            {
                var activeNaturalGasSellingPrice = GetActiveNaturalGasSellingPrice();
                activeNaturalGasSellingPrice.AmountCorrection(
                    command.Amount, command.Remark, command.Year, command.Month);
                _unitOfWork.Update(activeNaturalGasSellingPrice);
                LogNaturalGasSellingPriceCorrection(activeNaturalGasSellingPrice);

                return activeNaturalGasSellingPrice;
            }

            void CorrectCogenerationTariffs()
            {
                var previousRes = GetPreviousActiveCogenerationTariffsFor(correctedGsp.Period.ValidFrom);

                GetActiveCogenerationTariffsFor(correctedGsp).ToList().ForEach(act =>
                {
                    act.GspCorrection(
                        GetNaturalGasPricesWithinYear(command.Year),
                        _cogenerationParameterService,
                        correctedGsp,
                        PreviousActiveCogenerationTariffBy(act.ProjectTypeId));
                    _unitOfWork.Update(act);
                    LogNewCogenerationTariffCorrection(act);
                });

                CogenerationTariff PreviousActiveCogenerationTariffBy(Guid projectTypeId) =>
                    previousRes.Single(res => res.ProjectTypeId.Equals(projectTypeId));

                IReadOnlyList<NaturalGasSellingPrice> GetNaturalGasPricesWithinYear(int year) =>
                    _repository.GetAll(new NaturalGasSellingPricesInAYearSpecification(year));
            }
        }

        private NaturalGasSellingPrice GetActiveNaturalGasSellingPrice() =>
            _repository.GetLatest<NaturalGasSellingPrice>();

        private void LogNaturalGasSellingPriceCorrection(NaturalGasSellingPrice naturalGasSellingPrice) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    SubsidyMessages.ParameterCorrectionLog,
                    nameof(NaturalGasSellingPrice).Humanize(LetterCasing.LowerCase),
                    naturalGasSellingPrice.Period,
                    naturalGasSellingPrice.Amount)
            });

        private IReadOnlyList<CogenerationTariff> GetPreviousActiveCogenerationTariffsFor(DateTimeOffset validTill) =>
            _repository.GetAll(new ValidTillSpecification<CogenerationTariff>(validTill));

        private IReadOnlyList<CogenerationTariff> GetActiveCogenerationTariffsFor(NaturalGasSellingPrice gsp) =>
            _repository.GetAll(new GspCogenerationTariffSpecification(gsp.Id));

        private void LogNewCogenerationTariffCorrection(CogenerationTariff cogenerationTariff) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    SubsidyMessages.TariffCorrectionLog,
                    nameof(CogenerationTariff).Humanize(LetterCasing.LowerCase),
                    cogenerationTariff.Period,
                    cogenerationTariff.LowerRate,
                    cogenerationTariff.HigherRate)
            });
    }
}