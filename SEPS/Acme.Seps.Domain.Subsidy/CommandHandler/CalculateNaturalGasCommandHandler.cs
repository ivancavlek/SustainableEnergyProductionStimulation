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
    public sealed class CalculateNaturalGasCommandHandler
        : BaseCommandHandler, ISepsCommandHandler<CalculateNaturalGasCommand>
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly IRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CalculateNaturalGasCommandHandler(
            ICogenerationParameterService cogenerationParameterService,
            IRepository repository,
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
            var activeNgsp = GetActiveNaturalGasSellingPrice();
            var newNgsp = GetNewNaturalGasSellingPrice();
            CreateNewRenewableEnergySourceTariffs();

            _unitOfWork.Commit();
            LogSuccessfulCommit();

            NaturalGasSellingPrice GetNewNaturalGasSellingPrice()
            {
                var ngsp = CreateNewNaturalGasSellingPrice(activeNgsp, command);
                _unitOfWork.Update(activeNgsp);
                _unitOfWork.Insert(ngsp);
                LogNewNaturalGasSellingPriceCreation(ngsp);

                return ngsp;
            }

            void CreateNewRenewableEnergySourceTariffs()
            {
                var yearsNaturalGasSellingPrices = GetNaturalGasSellingPricesWithinYear(command.Year);

                GetActiveCogenerationTariffs().ToList()
                    .ForEach(ctf =>
                    {
                        var newCogenerationTariff =
                            CreateNewCogenerationTariff(ctf, yearsNaturalGasSellingPrices, newNgsp);
                        _unitOfWork.Update(ctf);
                        _unitOfWork.Insert(newCogenerationTariff);
                        LogNewCogenerationTariffCreation(newCogenerationTariff);
                    });
            }
        }

        private NaturalGasSellingPrice GetActiveNaturalGasSellingPrice() =>
            _repository.GetSingle(new ActiveSpecification<NaturalGasSellingPrice>());

        private NaturalGasSellingPrice CreateNewNaturalGasSellingPrice(
            NaturalGasSellingPrice naturalGasSellingPrice, CalculateNaturalGasCommand command) =>
            naturalGasSellingPrice.CreateNew(
                command.Amount, command.Remark, command.Month, command.Year, _identityFactory);

        private IReadOnlyList<CogenerationTariff> GetActiveCogenerationTariffs() =>
            _repository.GetAll(new ActiveSpecification<CogenerationTariff>());

        private IReadOnlyList<NaturalGasSellingPrice> GetNaturalGasSellingPricesWithinYear(int year) =>
           _repository.GetAll(new NaturalGasSellingPricesInAYearSpecification(year));

        private void LogNewNaturalGasSellingPriceCreation(NaturalGasSellingPrice naturalGasSellingPrice) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    SubsidyMessages.InsertParameterLog,
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
                    SubsidyMessages.InsertTariffLog,
                    nameof(CogenerationTariff).Humanize(LetterCasing.LowerCase),
                    cogenerationTariff.Period,
                    cogenerationTariff.LowerRate,
                    cogenerationTariff.HigherRate)
            });
    }
}