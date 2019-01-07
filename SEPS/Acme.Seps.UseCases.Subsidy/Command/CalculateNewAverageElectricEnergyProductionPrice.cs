using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Text;
using FluentValidation;
using System;

namespace Acme.Seps.UseCases.Subsidy.Command
{
    public sealed class CalculateNewAverageElectricEnergyProductionPriceCommandHandler
        : BaseCommandHandler, ISepsCommandHandler<CalculateNewAverageElectricEnergyProductionPriceCommand>
    {
        public CalculateNewAverageElectricEnergyProductionPriceCommandHandler(
            IRepository repository, IUnitOfWork unitOfWork, IIdentityFactory<Guid> identityFactory)
            : base(repository, unitOfWork, identityFactory) { }

        void ICommandHandler<CalculateNewAverageElectricEnergyProductionPriceCommand>.Handle(
            CalculateNewAverageElectricEnergyProductionPriceCommand command)
        {
            //var activeNgsp = GetActiveMonthlyAverageElectricEnergyProductionPrice();

            //var newNgsp = CreateNewMonthlyAverageElectricEnergyProductionPrice(activeNgsp, command);
            //CreateNewRenewableEnergySourceTariffs(newNgsp);

            //_unitOfWork.Update(activeNgsp);
            //_unitOfWork.Insert(newNgsp);
            //_unitOfWork.Commit();

            //LogNewNaturalGasSellingPriceCreation(newNgsp);
            //LogSuccessfulCommit();
        }

        //private MonthlyAverageElectricEnergyProductionPrice GetActiveMonthlyAverageElectricEnergyProductionPrice() =>
        //    _repository.GetSingle(new ActiveSpecification<MonthlyAverageElectricEnergyProductionPrice>());

        //private MonthlyAverageElectricEnergyProductionPrice CreateNewMonthlyAverageElectricEnergyProductionPrice(
        //    MonthlyAverageElectricEnergyProductionPrice monthlyAverageElectricEnergyProductionPrice,
        //    CalculateNewAverageElectricEnergyProductionPriceCommand command) =>
        //    monthlyAverageElectricEnergyProductionPrice.CreateNew(
        //        command.Amount, command.Remark, command.Month, command.Year, _identityFactory);

        //private void CreateNewRenewableEnergySourceTariffs(NaturalGasSellingPrice newNgsp)
        //{
        //    var yearlyAverageElectricEnergyProductionPrice = GetActiveYearlyAverageElectricEnergyProductionPrice();

        //    GetActiveCogenerationTariffs().ForEach(ctf =>
        //    {
        //        var newCogenerationTariff = CreateNewCogenerationTariff(ctf);

        //        _unitOfWork.Update(ctf);
        //        _unitOfWork.Insert(newCogenerationTariff);

        //        LogNewCogenerationTariffCreation(newCogenerationTariff);
        //    });

        //    CogenerationTariff CreateNewCogenerationTariff(CogenerationTariff cogenerationTariff) =>
        //        cogenerationTariff.CreateNewWith(
        //            _cogenerationParameterService, yearlyAverageElectricEnergyProductionPrice, newNgsp, _identityFactory);
        //}

        //private YearlyAverageElectricEnergyProductionPrice GetActiveYearlyAverageElectricEnergyProductionPrice() =>
        //   _repository.GetSingle(new ActiveSpecification<YearlyAverageElectricEnergyProductionPrice>());

        //private List<CogenerationTariff> GetActiveCogenerationTariffs() =>
        //    _repository.GetAll(new ActiveSpecification<CogenerationTariff>()).ToList();

        //private void LogNewCogenerationTariffCreation(CogenerationTariff cogenerationTariff) =>
        //    Log(new EntityExecutionLoggingEventArgs
        //    (
        //        SepsMessage.InsertTariff(
        //            nameof(CogenerationTariff),
        //            cogenerationTariff.Active.Since.Date,
        //            cogenerationTariff.Active.Until,
        //            cogenerationTariff.LowerRate,
        //            cogenerationTariff.HigherRate)
        //    ));

        //private void LogNewNaturalGasSellingPriceCreation(NaturalGasSellingPrice naturalGasSellingPrice) =>
        //    Log(new EntityExecutionLoggingEventArgs
        //    (
        //        SepsMessage.InsertParameter(
        //            nameof(NaturalGasSellingPrice),
        //            naturalGasSellingPrice.Active.Since.Date,
        //            naturalGasSellingPrice.Active.Until,
        //            naturalGasSellingPrice.Amount)
        //    ));
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