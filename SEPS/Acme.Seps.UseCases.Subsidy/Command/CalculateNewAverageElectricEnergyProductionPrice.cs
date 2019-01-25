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
    public sealed class CalculateNewAverageElectricEnergyProductionPriceCommandHandler
        : BaseCommandHandler, ISepsCommandHandler<CalculateNewAverageElectricEnergyProductionPriceCommand>
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;

        public CalculateNewAverageElectricEnergyProductionPriceCommandHandler(
            ICogenerationParameterService cogenerationParameterService,
            IRepository repository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
            : base(repository, unitOfWork, identityFactory)
        {
            _cogenerationParameterService = cogenerationParameterService ?? throw new ArgumentNullException(nameof(cogenerationParameterService));
        }

        void ICommandHandler<CalculateNewAverageElectricEnergyProductionPriceCommand>.Handle(
            CalculateNewAverageElectricEnergyProductionPriceCommand command)
        {
            var activeAeepp = GetActiveAverageElectricEnergyProductionPrice();
            var newAeepp = CreateNewAverageElectricEnergyProductionPrice(activeAeepp, command);

            CreateNewCogenerationTariffs(newAeepp);

            Commit();

            LogNewAverageElectricEnergyProductionPriceCreation(newAeepp);
            LogSuccessfulCommit();
        }

        private AverageElectricEnergyProductionPrice GetActiveAverageElectricEnergyProductionPrice() =>
            _repository.GetSingle(new ActiveSpecification<AverageElectricEnergyProductionPrice>());

        private AverageElectricEnergyProductionPrice CreateNewAverageElectricEnergyProductionPrice(
            AverageElectricEnergyProductionPrice activeAeepp,
            CalculateNewAverageElectricEnergyProductionPriceCommand command)
        {
            var newAeepp = activeAeepp.CreateNew(
                command.Amount, command.Remark, command.Month, command.Year, _identityFactory);

            _unitOfWork.Update(activeAeepp);
            _unitOfWork.Insert(newAeepp);

            return newAeepp;
        }

        private void CreateNewCogenerationTariffs(AverageElectricEnergyProductionPrice aeepp)
        {
            var ngsp = GetActiveNaturalGasSellingPrice();

            GetActiveCogenerationTariffs().ForEach(ctf =>
            {
                var newCogenerationTariff = CreateNewCogenerationTariff(ctf);

                _unitOfWork.Update(ctf);
                _unitOfWork.Insert(newCogenerationTariff);

                LogNewCogenerationTariffCreation(newCogenerationTariff);
            });

            CogenerationTariff CreateNewCogenerationTariff(CogenerationTariff cogenerationTariff) =>
                cogenerationTariff.CreateNewWith(
                    _cogenerationParameterService, aeepp, ngsp, aeepp.Active.Since, _identityFactory);
        }

        private NaturalGasSellingPrice GetActiveNaturalGasSellingPrice() =>
           _repository.GetSingle(new ActiveSpecification<NaturalGasSellingPrice>());

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

        private void LogNewAverageElectricEnergyProductionPriceCreation(
            AverageElectricEnergyProductionPrice averageElectricEnergyProductionPrice) =>
            Log(new EntityExecutionLoggingEventArgs
            (
                SepsMessage.InsertParameter(
                    nameof(NaturalGasSellingPrice),
                    averageElectricEnergyProductionPrice.Active.Since.Date,
                    averageElectricEnergyProductionPrice.Active.Until,
                    averageElectricEnergyProductionPrice.Amount)
            ));
    }

    public sealed class CalculateNewAverageElectricEnergyProductionPriceCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}