using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.DomainService;
using Acme.Seps.Domain.Parameter.Entity;
using Acme.Seps.Domain.Parameter.Repository;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.CommandHandler
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
            var activeNaturalGasSellingPrice = GetActiveNaturalGasSellingPrice();
            //activeNaturalGasSellingPrice.GspCorrection(command.Amount, command.Remark, command.Year, command.Month);
            _unitOfWork.Update(activeNaturalGasSellingPrice);
            LogNaturalGasSellingPriceCorrection(activeNaturalGasSellingPrice);

            GetActiveCogenerationTariffsFor(activeNaturalGasSellingPrice).ToList().ForEach(act =>
            {
                // get by type previous from active
                // type.GspCorrection(
                _unitOfWork.Update(act);
                LogNewCogenerationTariffCorrection(act);
            });

            _unitOfWork.Commit();
            LogSuccessfulCommit();
        }

        private NaturalGasSellingPrice GetActiveNaturalGasSellingPrice() =>
            _repository.GetLatest<NaturalGasSellingPrice>();

        private void LogNaturalGasSellingPriceCorrection(NaturalGasSellingPrice naturalGasSellingPrice) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.ParameterCorrectionLog,
                    nameof(NaturalGasSellingPrice).Humanize(LetterCasing.LowerCase),
                    naturalGasSellingPrice.Period,
                    naturalGasSellingPrice.Amount)
            });

        private IReadOnlyList<CogenerationTariff> GetActiveCogenerationTariffsFor(NaturalGasSellingPrice gsp) =>
            _repository.GetAll(new GspCogenerationTariffSpecification(gsp.Id));

        private CogenerationTariff CreateNewRenewableEnergySourceTariff(
            CogenerationTariff cogenerationTariff,
            NaturalGasSellingPrice naturalGasSellingPrice,
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices) =>
            cogenerationTariff.CreateNewWith(
                yearsNaturalGasSellingPrices,
                _cogenerationParameterService,
                naturalGasSellingPrice,
                _identityFactory);

        private void LogNewCogenerationTariffCorrection(CogenerationTariff cogenerationTariff) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.TariffCorrectionLog,
                    nameof(CogenerationTariff).Humanize(LetterCasing.LowerCase),
                    cogenerationTariff.Period,
                    cogenerationTariff.LowerRate,
                    cogenerationTariff.HigherRate)
            });
    }
}