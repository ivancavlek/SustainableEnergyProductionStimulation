using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Entity;
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
        private readonly IRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CorrectActiveNaturalGasCommandHandler(
            ICogenerationParameterService cogenerationParameterService,
            IRepository repository,
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

        void ICommandHandler<CorrectActiveNaturalGasCommand>.Handle(CorrectActiveNaturalGasCommand command)
        {
            DateTimeOffset oldActiveFrom;
            var correctedGsp = CorrectActiveGsp();
            CorrectCogenerationTariffs();

            _unitOfWork.Commit();
            LogSuccessfulCommit();

            NaturalGasSellingPrice CorrectActiveGsp()
            {
                var activeNaturalGasSellingPrice = GetActiveNaturalGasSellingPrice();
                oldActiveFrom = activeNaturalGasSellingPrice.Period.ActiveFrom;
                var previousActiveNaturalGasSellingPrice = GetPreviousActiveNaturalGasSellingPrice(oldActiveFrom);

                activeNaturalGasSellingPrice.Correct(
                    command.Amount, command.Remark, command.Year, command.Month, previousActiveNaturalGasSellingPrice);

                _unitOfWork.Update(activeNaturalGasSellingPrice);
                _unitOfWork.Update(previousActiveNaturalGasSellingPrice);

                LogNaturalGasSellingPriceCorrection(activeNaturalGasSellingPrice);

                return activeNaturalGasSellingPrice;
            }

            void CorrectCogenerationTariffs()
            {
                var previousRes = GetPreviousActiveCogenerationTariffs(oldActiveFrom);

                GetActiveCogenerationTariffsFor().ToList().ForEach(act =>
                {
                    var previouRes = PreviousActiveCogenerationTariffBy(act.ProjectTypeId);

                    act.NgspCorrection(
                        GetNaturalGasPricesWithinYear(command.Year),
                        _cogenerationParameterService,
                        correctedGsp,
                        previouRes);

                    _unitOfWork.Update(act);
                    _unitOfWork.Update(previouRes);

                    LogNewCogenerationTariffCorrection(act);
                });

                CogenerationTariff PreviousActiveCogenerationTariffBy(Guid projectTypeId) =>
                    previousRes.Single(res => res.ProjectTypeId.Equals(projectTypeId));

                IReadOnlyList<NaturalGasSellingPrice> GetNaturalGasPricesWithinYear(int year) =>
                    _repository.GetAll(new NaturalGasSellingPricesInAYearSpecification(year));
            }
        }

        private NaturalGasSellingPrice GetActiveNaturalGasSellingPrice() =>
            _repository.GetSingle(new ActiveSpecification<NaturalGasSellingPrice>());

        private NaturalGasSellingPrice GetPreviousActiveNaturalGasSellingPrice(DateTimeOffset activeTill) =>
            _repository.GetSingle(new PreviousActiveSpecification<NaturalGasSellingPrice>(activeTill));

        private void LogNaturalGasSellingPriceCorrection(NaturalGasSellingPrice naturalGasSellingPrice) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    SubsidyMessages.ParameterCorrectionLog,
                    nameof(NaturalGasSellingPrice).Humanize(LetterCasing.LowerCase),
                    naturalGasSellingPrice.Period,
                    naturalGasSellingPrice.Amount)
            });

        private IReadOnlyList<CogenerationTariff> GetPreviousActiveCogenerationTariffs(DateTimeOffset activeTill) =>
            _repository.GetAll(new PreviousActiveSpecification<CogenerationTariff>(activeTill));

        private IReadOnlyList<CogenerationTariff> GetActiveCogenerationTariffsFor() =>
            _repository.GetAll(new ActiveSpecification<CogenerationTariff>());

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