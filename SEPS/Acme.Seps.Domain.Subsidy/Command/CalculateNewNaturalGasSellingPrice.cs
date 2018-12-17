using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Subsidy.Command.DomainService;
using Acme.Seps.Domain.Subsidy.Command.Entity;
using Acme.Seps.Domain.Subsidy.Command.Infrastructure;
using Acme.Seps.Domain.Subsidy.Command.Repository;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Subsidy.Command
{
    public sealed class CalculateNaturalGasSellingPriceCommandHandler
        : BaseCommandHandler, ISepsCommandHandler<CalculateNaturalGasSellingPriceCommand>
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly IRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CalculateNaturalGasSellingPriceCommandHandler(
            ICogenerationParameterService cogenerationParameterService,
            IRepository repository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
        {
            _cogenerationParameterService = cogenerationParameterService ?? throw new ArgumentNullException(nameof(cogenerationParameterService));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        void ICommandHandler<CalculateNaturalGasSellingPriceCommand>.Handle(
            CalculateNaturalGasSellingPriceCommand command)
        {
            var activeNgsp = GetActiveNaturalGasSellingPrice();

            var newNgsp = CreateNewNaturalGasSellingPrice(activeNgsp, command);
            CreateNewRenewableEnergySourceTariffs(newNgsp);

            _unitOfWork.Update(activeNgsp);
            _unitOfWork.Insert(newNgsp);
            _unitOfWork.Commit();

            LogNewNaturalGasSellingPriceCreation(newNgsp);
            LogSuccessfulCommit();
        }

        private NaturalGasSellingPrice GetActiveNaturalGasSellingPrice() =>
            _repository.GetSingle(new ActiveSpecification<NaturalGasSellingPrice>());

        private NaturalGasSellingPrice CreateNewNaturalGasSellingPrice(
            NaturalGasSellingPrice naturalGasSellingPrice, CalculateNaturalGasSellingPriceCommand command) =>
            naturalGasSellingPrice.CreateNew(
                command.Amount, command.Remark, command.Month, command.Year, _identityFactory);

        private void CreateNewRenewableEnergySourceTariffs(NaturalGasSellingPrice newNgsp)
        {
            var yearsNaturalGasSellingPrices = GetNaturalGasSellingPricesWithinYear(newNgsp.Active.Since.Year);

            GetActiveCogenerationTariffs().ForEach(ctf =>
            {
                var newCogenerationTariff = CreateNewCogenerationTariff(ctf);

                _unitOfWork.Update(ctf);
                _unitOfWork.Insert(newCogenerationTariff);

                LogNewCogenerationTariffCreation(newCogenerationTariff);
            });

            CogenerationTariff CreateNewCogenerationTariff(CogenerationTariff cogenerationTariff) =>
                cogenerationTariff.CreateNewWith(
                    yearsNaturalGasSellingPrices, _cogenerationParameterService, newNgsp, _identityFactory);
        }

        private IReadOnlyList<NaturalGasSellingPrice> GetNaturalGasSellingPricesWithinYear(int year) =>
           _repository.GetAll(new NaturalGasSellingPricesInAYearSpecification(year));

        private List<CogenerationTariff> GetActiveCogenerationTariffs() =>
            _repository.GetAll(new ActiveSpecification<CogenerationTariff>()).ToList();

        private void LogNewCogenerationTariffCreation(CogenerationTariff cogenerationTariff) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    SubsidyMessages.InsertTariffLog,
                    nameof(CogenerationTariff).Humanize(LetterCasing.LowerCase),
                    cogenerationTariff.Active,
                    cogenerationTariff.LowerRate,
                    cogenerationTariff.HigherRate)
            });

        private void LogNewNaturalGasSellingPriceCreation(NaturalGasSellingPrice naturalGasSellingPrice) =>
            Log(new EntityExecutionLoggingEventArgs
            {
                Message = string.Format(
                    SubsidyMessages.InsertParameterLog,
                    nameof(NaturalGasSellingPrice).Humanize(LetterCasing.LowerCase),
                    naturalGasSellingPrice.Active,
                    naturalGasSellingPrice.Amount)
            });
    }

    public sealed class CalculateNaturalGasSellingPriceCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}