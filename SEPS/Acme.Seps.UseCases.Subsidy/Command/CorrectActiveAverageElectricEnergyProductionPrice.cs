using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base;
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
    public sealed class CorrectActiveAverageElectricEnergyProductionPriceCommandHandler
        : BaseCommandHandler, ISepsCommandHandler<CorrectActiveAverageElectricEnergyProductionPriceCommand>
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;

        public CorrectActiveAverageElectricEnergyProductionPriceCommandHandler(
            ICogenerationParameterService cogenerationParameterService,
            IRepository repository,
            IUnitOfWork unitOfWork,
            IIdentityFactory<Guid> identityFactory)
            : base(repository, unitOfWork, identityFactory)
        {
            _cogenerationParameterService =
                cogenerationParameterService ?? throw new ArgumentNullException(nameof(cogenerationParameterService));
        }

        void ICommandHandler<CorrectActiveAverageElectricEnergyProductionPriceCommand>.Handle(
            CorrectActiveAverageElectricEnergyProductionPriceCommand command)
        {
            var activeAverageElectricEnergyProductionPrice = GetActiveAverageElectricEnergyProductionPrice();
            var previousActiveAverageElectricEnergyProductionPrice =
                GetPreviousActiveAverageElectricEnergyProductionPrice(activeAverageElectricEnergyProductionPrice);
            var previousCogenerations = GetPreviousActiveCogenerationTariffs(activeAverageElectricEnergyProductionPrice);

            activeAverageElectricEnergyProductionPrice.Correct(
                command.Amount, command.Remark, command.Year, command.Month, previousActiveAverageElectricEnergyProductionPrice);
            CorrectCogenerationTariffs(activeAverageElectricEnergyProductionPrice, previousCogenerations);

            _unitOfWork.Update(activeAverageElectricEnergyProductionPrice);
            _unitOfWork.Update(previousActiveAverageElectricEnergyProductionPrice);
            Commit();

            LogAverageElectricEnergyProductionPriceCorrection(activeAverageElectricEnergyProductionPrice);
            LogSuccessfulCommit();
        }

        private AverageElectricEnergyProductionPrice GetActiveAverageElectricEnergyProductionPrice() =>
            _repository.GetSingle(new ActiveSpecification<AverageElectricEnergyProductionPrice>());

        private AverageElectricEnergyProductionPrice GetPreviousActiveAverageElectricEnergyProductionPrice(
            AverageElectricEnergyProductionPrice aeepp) =>
            _repository.GetSingle(new PreviousActiveSpecification<AverageElectricEnergyProductionPrice>(aeepp));

        private IReadOnlyList<CogenerationTariff> GetPreviousActiveCogenerationTariffs(
            AverageElectricEnergyProductionPrice aeepp) =>
            _repository.GetAll(new PreviousActiveSpecification<CogenerationTariff>(aeepp));

        private void CorrectCogenerationTariffs(
            AverageElectricEnergyProductionPrice correctedAeepp, IEnumerable<CogenerationTariff> previousCogenerations)
        {
            GetActiveCogenerationTariffs().ForEach(ctf =>
            {
                var previousCogeneration = CogenerationTariffByProjectType(ctf.ProjectTypeId);

                ctf.NaturalGasSellingPriceCorrection(
                    _cogenerationParameterService,
                    correctedAeepp,
                    GetActiveNaturalGasSellingPrice(),
                    previousCogeneration);

                _unitOfWork.Update(ctf);
                _unitOfWork.Update(previousCogeneration);

                LogNewCogenerationTariffCorrection(ctf);
            });

            CogenerationTariff CogenerationTariffByProjectType(Guid projectTypeId) =>
                previousCogenerations.Single(ctf => ctf.ProjectTypeId.Equals(projectTypeId));
        }

        private List<CogenerationTariff> GetActiveCogenerationTariffs() =>
            _repository.GetAll(new ActiveSpecification<CogenerationTariff>()).ToList();

        private NaturalGasSellingPrice GetActiveNaturalGasSellingPrice() =>
            _repository.GetSingle(new ActiveSpecification<NaturalGasSellingPrice>());

        private void LogNewCogenerationTariffCorrection(CogenerationTariff cogenerationTariff) =>
            Log(new EntityExecutionLoggingEventArgs
            (
                SepsMessage.TariffCorrection(
                    nameof(CogenerationTariff),
                    cogenerationTariff.Active.Since.Date,
                    cogenerationTariff.Active.Until,
                    cogenerationTariff.LowerRate,
                    cogenerationTariff.HigherRate)
            ));

        private void LogAverageElectricEnergyProductionPriceCorrection(
            AverageElectricEnergyProductionPrice aeepp) =>
            Log(new EntityExecutionLoggingEventArgs
            (
                SepsMessage.ParameterCorrection(
                    nameof(AverageElectricEnergyProductionPrice),
                    aeepp.Active.Since.Date,
                    aeepp.Active.Until,
                    aeepp.Amount)
            ));
    }

    public sealed class CorrectActiveAverageElectricEnergyProductionPriceCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}