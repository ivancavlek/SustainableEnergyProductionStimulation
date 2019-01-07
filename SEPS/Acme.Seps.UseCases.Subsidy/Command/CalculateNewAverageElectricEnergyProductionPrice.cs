using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Text;
using FluentValidation;
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
            var activeMaeepp = GetActiveMonthlyAverageElectricEnergyProductionPrice();
            var newMaeepp = CreateNewMonthlyAverageElectricEnergyProductionPrice(activeMaeepp, command);

            var yaeepp = YearlyAverageElectricEnergyProductionPriceCalculation(newMaeepp);
            CreateNewCogenerationTariffs(yaeepp);

            Commit();

            LogNewAverageElectricEnergyProductionPriceCreation(newMaeepp);
            LogSuccessfulCommit();
        }

        private MonthlyAverageElectricEnergyProductionPrice GetActiveMonthlyAverageElectricEnergyProductionPrice() =>
            _repository.GetSingle(new ActiveSpecification<MonthlyAverageElectricEnergyProductionPrice>());

        private MonthlyAverageElectricEnergyProductionPrice CreateNewMonthlyAverageElectricEnergyProductionPrice(
            MonthlyAverageElectricEnergyProductionPrice activeMaeepp,
            CalculateNewAverageElectricEnergyProductionPriceCommand command)
        {
            var newMaeepp = activeMaeepp.CreateNew(
                command.Amount, command.Remark, command.Month, command.Year, _identityFactory);

            _unitOfWork.Update(activeMaeepp);
            _unitOfWork.Insert(newMaeepp);

            return newMaeepp;
        }

        private void CreateNewCogenerationTariffs(YearlyAverageElectricEnergyProductionPrice yaeepp)
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
                cogenerationTariff.CreateNewWith(_cogenerationParameterService, yaeepp, ngsp, _identityFactory);
        }

        private YearlyAverageElectricEnergyProductionPrice GetActiveYearlyAverageElectricEnergyProductionPrice() =>
           _repository.GetSingle(new ActiveSpecification<YearlyAverageElectricEnergyProductionPrice>());

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
            MonthlyAverageElectricEnergyProductionPrice monthlyAverageElectricEnergyProductionPrice) =>
            Log(new EntityExecutionLoggingEventArgs
            (
                SepsMessage.InsertParameter(
                    nameof(NaturalGasSellingPrice),
                    monthlyAverageElectricEnergyProductionPrice.Active.Since.Date,
                    monthlyAverageElectricEnergyProductionPrice.Active.Until,
                    monthlyAverageElectricEnergyProductionPrice.Amount)
            ));
    }

    public sealed class CalculateNewAverageElectricEnergyProductionPriceCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public sealed class CalculateNewAverageElectricEnergyProductionPriceCommandValidator
        : AbstractValidator<CalculateNewAverageElectricEnergyProductionPriceCommand>
    {
        public CalculateNewAverageElectricEnergyProductionPriceCommandValidator()
        {
            RuleFor(cng => cng.Amount)
                .GreaterThan(0M)
                .WithMessage(cng => SepsMessage.ValueZeroOrAbove(nameof(cng.Amount)));
            RuleFor(cng => cng.Remark)
                .NotEmpty()
                .WithMessage(cng => SepsMessage.EntityNotSet(nameof(cng.Remark)));
            RuleFor(cng => cng.Year)
                .GreaterThan(SepsVersion.InitialDate().Year)
                .WithMessage(cng => SepsMessage.ValueHigherThanTheOther(cng.Year.ToString(), SepsVersion.InitialDate().Year.ToString()));
            RuleFor(cng => cng.Month)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(12)
                .WithMessage(cng => SepsMessage.ValueHigherThanTheOther(cng.Month.ToString(), "1 - 12"));
        }
    }
}