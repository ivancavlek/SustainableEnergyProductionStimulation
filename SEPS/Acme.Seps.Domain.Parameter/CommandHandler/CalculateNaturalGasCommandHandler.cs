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
    public sealed class CalculateNaturalGasCommandHandler
        : BaseCommandHandler, ISepsCommandHandler<CalculateNaturalGasCommand>
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly ISepsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CalculateNaturalGasCommandHandler(
            ICogenerationParameterService cogenerationParameterService,
            ISepsRepository repository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
        {
            _cogenerationParameterService = cogenerationParameterService ?? throw new ArgumentNullException(nameof(cogenerationParameterService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        void ICommandHandler<CalculateNaturalGasCommand>.Handle(CalculateNaturalGasCommand command)
        {
            var activeNaturalGasSellingPrice = GetActiveNaturalGasSellingPrice();
            var newNaturalGasSellingPrice = CreateNewNaturalGasSellingPrice(activeNaturalGasSellingPrice, command);
            _unitOfWork.Update(activeNaturalGasSellingPrice);
            _unitOfWork.Insert(newNaturalGasSellingPrice);
            LogNewNaturalSellingPriceCreation(newNaturalGasSellingPrice);

            var yearsNaturalGasSellingPrices = GetNaturalGasPricesWithinYear(command.Year);

            GetActiveCogenerations(activeNaturalGasSellingPrice.Id).ToList()
                .ForEach(ctf =>
                {
                    var newCogenerationTariff =
                        CreateNewCogenerationTariff(ctf, yearsNaturalGasSellingPrices, newNaturalGasSellingPrice);
                    _unitOfWork.Update(ctf);
                    _unitOfWork.Insert(newCogenerationTariff);
                    LogNewCogenerationTariffCreation(newCogenerationTariff);
                });

            _unitOfWork.Commit();
            LogSuccessfulCommit();
        }

        private NaturalGasSellingPrice GetActiveNaturalGasSellingPrice() =>
            _repository.GetLatest<NaturalGasSellingPrice>();

        private NaturalGasSellingPrice CreateNewNaturalGasSellingPrice(
            NaturalGasSellingPrice naturalGasSellingPrice, CalculateNaturalGasCommand command) =>
            naturalGasSellingPrice.CreateNew(
                command.Amount, command.Remark, command.Month, command.Year, _identityFactory);

        private IReadOnlyList<CogenerationTariff> GetActiveCogenerations(Guid gspId) =>
            _repository.GetAll(new GspCogenerationTariffSpecification(gspId));

        private IReadOnlyList<NaturalGasSellingPrice> GetNaturalGasPricesWithinYear(int year) =>
           _repository.GetAll(new NaturalGasSellingPricesInAYearSpecification(year));

        private void LogNewNaturalSellingPriceCreation(NaturalGasSellingPrice naturalGasSellingPrice) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    Infrastructure.Parameter.InsertParameterLog,
                    nameof(NaturalGasSellingPrice).Humanize(LetterCasing.LowerCase),
                    naturalGasSellingPrice.Period,
                    naturalGasSellingPrice.Amount)
            });

        private CogenerationTariff CreateNewCogenerationTariff(
            CogenerationTariff cogenerationTariff,
            IEnumerable<NaturalGasSellingPrice> naturalGasSellingPrices,
            NaturalGasSellingPrice naturalGasSellingPrice) =>
            cogenerationTariff.CreateNewWith(
                naturalGasSellingPrices,
                _cogenerationParameterService,
                naturalGasSellingPrice,
                _identityFactory);

        private void LogNewCogenerationTariffCreation(CogenerationTariff cogenerationTariff) =>
            Log(new EntityExecutionLoggingEventArgs
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