using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ApplicationService;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.DomainService;
using Acme.Seps.Domain.Parameter.Entity;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.CommandHandler
{
    public sealed class CorrectActiveNaturalGasCommandHandler
        : BaseCommandHandler, ICommandHandler<CorrectActiveNaturalGasCommand>
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CorrectActiveNaturalGasCommandHandler(
            ICogenerationParameterService cogenerationParameterService,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory,
            ISepsLogService sepsLogService) : base(sepsLogService)
        {
            _cogenerationParameterService = cogenerationParameterService ?? throw new ArgumentNullException(nameof(cogenerationParameterService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        void ICommandHandler<CorrectActiveNaturalGasCommand>.Handle(
            CorrectActiveNaturalGasCommand command)
        {
            var newNaturalGasSellingPrice = CreateNewNaturalGasSellingPrice(command);
            _unitOfWork.Insert(newNaturalGasSellingPrice);
            _unitOfWork.Delete(newNaturalGasSellingPrice);
            LogNaturalGasSellingPriceUpdate(newNaturalGasSellingPrice);

            command.ActiveCogenerationTariffs.ToList().ForEach(act =>
            {
                var newCogenerationTariff = CreateNewRenewableEnergySourceTariff(
                    act, newNaturalGasSellingPrice, command.YearsNaturalGasSellingPrices);
                _unitOfWork.Insert(newCogenerationTariff);
                _unitOfWork.Delete(newCogenerationTariff);
                LogNewCogenerationTariffCreation(newCogenerationTariff);
            });

            _unitOfWork.Commit();
            LogSuccessfulCommit();
        }

        private NaturalGasSellingPrice CreateNewNaturalGasSellingPrice(CorrectActiveNaturalGasCommand command) =>
            command.ActiveNaturalGasSellingPrice.CreateNew(
                command.Amount, command.Remark, command.Month, command.Year, _identityFactory)
            as NaturalGasSellingPrice;

        private void LogNaturalGasSellingPriceUpdate(NaturalGasSellingPrice naturalGasSellingPrice) =>
            SepsLogService.Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.InsertParameterLog,
                    nameof(NaturalGasSellingPrice).Humanize(LetterCasing.LowerCase),
                    naturalGasSellingPrice.Period,
                    naturalGasSellingPrice.Amount)
            });

        private CogenerationTariff CreateNewRenewableEnergySourceTariff(
            CogenerationTariff cogenerationTariff,
            NaturalGasSellingPrice naturalGasSellingPrice,
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices) =>
            cogenerationTariff.CreateNewWith(
                yearsNaturalGasSellingPrices,
                _cogenerationParameterService,
                naturalGasSellingPrice,
                _identityFactory);

        private void LogNewCogenerationTariffCreation(CogenerationTariff cogenerationTariff) =>
            SepsLogService.Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.InsertTariffLog,
                    nameof(CogenerationTariff).Humanize(LetterCasing.LowerCase),
                    cogenerationTariff.Period,
                    cogenerationTariff.LowerRate,
                    cogenerationTariff.HigherRate)
            });
    }
}