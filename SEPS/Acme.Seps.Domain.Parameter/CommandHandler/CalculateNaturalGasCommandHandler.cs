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
using System.Linq;

namespace Acme.Seps.Domain.Parameter.CommandHandler
{
    public sealed class CalculateNaturalGasCommandHandler : BaseCommandHandler, ICommandHandler<CalculateNaturalGasCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;
        private readonly ICogenerationParameterService _cogenerationParameterService;

        public CalculateNaturalGasCommandHandler(
            ICogenerationParameterService cogenerationParameterService,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory,
            ISepsLogService sepsLogService) : base(sepsLogService)
        {
            _cogenerationParameterService = cogenerationParameterService ?? throw new ArgumentNullException(nameof(cogenerationParameterService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        void ICommandHandler<CalculateNaturalGasCommand>.Handle(CalculateNaturalGasCommand command)
        {
            var newNaturalGasSellingPrice = CreateNewNaturalGasSellingPrice(command);
            _unitOfWork.Insert(newNaturalGasSellingPrice);
            LogNewNaturalSellingPriceCreation(newNaturalGasSellingPrice);

            command.ActiveCogenerationTariffs.ToList()
                .ForEach(ctf =>
                {
                    var newCogenerationTariff = CreateNewCogenerationTariff(ctf, command, newNaturalGasSellingPrice);
                    _unitOfWork.Insert(newCogenerationTariff);
                    LogNewCogenerationTariffCreation(newCogenerationTariff);
                });

            _unitOfWork.Commit();
            LogSuccessfulCommit();
        }

        private NaturalGasSellingPrice CreateNewNaturalGasSellingPrice(CalculateNaturalGasCommand command) =>
            command.ActiveNaturalGasSellingPrice.CreateNew(
                command.Amount, command.Remark, command.Month, command.Year, _identityFactory)
            as NaturalGasSellingPrice;

        private void LogNewNaturalSellingPriceCreation(NaturalGasSellingPrice naturalGasSellingPrice) =>
            SepsLogService.Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.InsertParameterLog,
                    nameof(NaturalGasSellingPrice).Humanize(LetterCasing.LowerCase),
                    naturalGasSellingPrice.Period,
                    naturalGasSellingPrice.Amount)
            });

        private CogenerationTariff CreateNewCogenerationTariff(
            CogenerationTariff cogenerationTariff,
            CalculateNaturalGasCommand command,
            NaturalGasSellingPrice naturalGasSellingPrice) =>
            cogenerationTariff.CreateNewWith(
                command.YearsNaturalGasSellingPrices,
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