using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ApplicationService;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.Entity;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.CommandHandler
{
    public sealed class CalculateCpiCommandHandler : BaseCommandHandler, ICommandHandler<CalculateCpiCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;
        private readonly IRepository<ConsumerPriceIndex> _cpiRepository;
        private readonly IRepository<RenewableEnergySourceTariff> _resRepository;

        public CalculateCpiCommandHandler(
            IRepository<ConsumerPriceIndex> cpiRepository,
            IRepository<RenewableEnergySourceTariff> resRepository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory,
            ISepsLogService sepsLogService) : base(sepsLogService)
        {
            _cpiRepository = cpiRepository ?? throw new ArgumentNullException(nameof(cpiRepository));
            _resRepository = resRepository ?? throw new ArgumentNullException(nameof(resRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
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
            GetActiveCpi().CreateNew(command.Amount, command.Remark, _identityFactory) as ConsumerPriceIndex;

        private ConsumerPriceIndex GetActiveCpi() =>
            _cpiRepository.Get(new ActiveSpecification<ConsumerPriceIndex>()).SingleOrDefault();

        private IEnumerable<RenewableEnergySourceTariff> GetActiveRes() =>
            _resRepository.Get(new ActiveSpecification<RenewableEnergySourceTariff>());

        private void LogNewNaturalSellingPriceCreation(ConsumerPriceIndex cpi) =>
            SepsLogService.Log(new EntityExecutionLoggingEventArgs
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