using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.UseCases.Subsidy.Command
{
    public sealed class CalculateNaturalGasSellingPriceCommandHandler
        : BaseCommandHandler, ISepsCommandHandler<CalculateNewNaturalGasSellingPriceCommand>
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;

        public CalculateNaturalGasSellingPriceCommandHandler(
            ICogenerationParameterService cogenerationParameterService,
            IRepository repository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
            : base(repository, unitOfWork, identityFactory)
        {
            _cogenerationParameterService = cogenerationParameterService ?? throw new ArgumentNullException(nameof(cogenerationParameterService));
        }

        void ICommandHandler<CalculateNewNaturalGasSellingPriceCommand>.Handle(
            CalculateNewNaturalGasSellingPriceCommand command)
        {
            var activeNgsp = GetActiveNaturalGasSellingPrice();

            var newNgsp = CreateNewNaturalGasSellingPrice(activeNgsp, command);
            CreateNewCogenerationTariffs(newNgsp);

            Commit();

            LogNewNaturalGasSellingPriceCreation(newNgsp);
            LogSuccessfulCommit();
        }

        private NaturalGasSellingPrice GetActiveNaturalGasSellingPrice() =>
            _repository.GetSingle(new ActiveSpecification<NaturalGasSellingPrice>());

        private NaturalGasSellingPrice CreateNewNaturalGasSellingPrice(
            NaturalGasSellingPrice activeNgsp, CalculateNewNaturalGasSellingPriceCommand command)
        {
            var newNgsp = activeNgsp.CreateNew(
                command.Amount, command.Remark, command.Month, command.Year, _identityFactory);

            _unitOfWork.Update(activeNgsp);
            _unitOfWork.Insert(newNgsp);

            return newNgsp;
        }

        private void CreateNewCogenerationTariffs(NaturalGasSellingPrice ngsp)
        {
            var activeAeepp = GetActiveAverageElectricEnergyProductionPrice();

            GetActiveCogenerationTariffs().ForEach(ctf =>
            {
                var newCogenerationTariff = CreateNewCogenerationTariff(ctf);

                _unitOfWork.Update(ctf);
                _unitOfWork.Insert(newCogenerationTariff);

                LogNewCogenerationTariffCreation(newCogenerationTariff);
            });

            CogenerationTariff CreateNewCogenerationTariff(CogenerationTariff cogenerationTariff) =>
                cogenerationTariff.CreateNewWith(
                    _cogenerationParameterService, activeAeepp, ngsp, ngsp.Active.Since, _identityFactory);
        }

        private AverageElectricEnergyProductionPrice GetActiveAverageElectricEnergyProductionPrice() =>
           _repository.GetSingle(new ActiveSpecification<AverageElectricEnergyProductionPrice>());

        private List<CogenerationTariff> GetActiveCogenerationTariffs() =>
            _repository.GetAll(new ActiveSpecification<CogenerationTariff>()).ToList();

        private void LogNewCogenerationTariffCreation(CogenerationTariff cogenerationTariff) =>
            Log(new EntityExecutionLoggingEventArgs
            (
                SepsMessage.InsertTariff(
                    nameof(CogenerationTariff),
                    cogenerationTariff.Active.Since.Date,
                    cogenerationTariff.Active.Until,
                    cogenerationTariff.LowerRate,
                    cogenerationTariff.HigherRate)
            ));

        private void LogNewNaturalGasSellingPriceCreation(NaturalGasSellingPrice naturalGasSellingPrice) =>
            Log(new EntityExecutionLoggingEventArgs
            (
                SepsMessage.InsertParameter(
                    nameof(NaturalGasSellingPrice),
                    naturalGasSellingPrice.Active.Since.Date,
                    naturalGasSellingPrice.Active.Until,
                    naturalGasSellingPrice.Amount)
            ));
    }

    public sealed class CalculateNewNaturalGasSellingPriceCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}