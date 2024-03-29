﻿using Acme.Domain.Base.CommandHandler;
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

namespace Acme.Seps.UseCases.Subsidy.Command;

public sealed class CorrectActiveNaturalGasSellingPriceCommandHandler
    : BaseCommandHandler, ISepsCommandHandler<CorrectActiveNaturalGasSellingPriceCommand>
{
    private readonly ICogenerationParameterService _cogenerationParameterService;

    public CorrectActiveNaturalGasSellingPriceCommandHandler(
        ICogenerationParameterService cogenerationParameterService,
        IRepository repository,
        IUnitOfWork unitOfWork,
        IIdentityFactory<Guid> identityFactory)
        : base(repository, unitOfWork, identityFactory) => _cogenerationParameterService =
            cogenerationParameterService ?? throw new ArgumentNullException(nameof(cogenerationParameterService));

    void ICommandHandler<CorrectActiveNaturalGasSellingPriceCommand>.Handle(
        CorrectActiveNaturalGasSellingPriceCommand command)
    {
        var activeNaturalGasSellingPrice = GetActiveNaturalGasSellingPrice();
        var previousActiveNaturalGasSellingPrice =
            GetPreviousActiveNaturalGasSellingPrice(activeNaturalGasSellingPrice);
        var previousCogenerations = GetPreviousActiveCogenerationTariffs(activeNaturalGasSellingPrice);

        activeNaturalGasSellingPrice.Correct(
            command.Amount, command.Remark, command.Year, command.Month, previousActiveNaturalGasSellingPrice);
        CorrectCogenerationTariffs(activeNaturalGasSellingPrice, previousCogenerations);

        _unitOfWork.Update(activeNaturalGasSellingPrice);
        _unitOfWork.Update(previousActiveNaturalGasSellingPrice);
        Commit();

        LogNaturalGasSellingPriceCorrection(activeNaturalGasSellingPrice);
        LogSuccessfulCommit();
    }

    private NaturalGasSellingPrice GetActiveNaturalGasSellingPrice() =>
        _repository.GetSingle(new ActiveSpecification<NaturalGasSellingPrice>());

    private NaturalGasSellingPrice GetPreviousActiveNaturalGasSellingPrice(NaturalGasSellingPrice ngsp) =>
        _repository.GetSingle(new PreviousActiveSpecification<NaturalGasSellingPrice>(ngsp));

    private IReadOnlyList<CogenerationTariff> GetPreviousActiveCogenerationTariffs(NaturalGasSellingPrice ngsp)
    {
        var specification = new PreviousActiveSpecification<CogenerationTariff>(ngsp);

        specification.AddInclude(ice => ice.AverageElectricEnergyProductionPrice);
        specification.AddInclude(ice => ice.NaturalGasSellingPrice);

        return _repository.GetAll(specification);
    }

    private void CorrectCogenerationTariffs(
        NaturalGasSellingPrice correctedNgsp, IEnumerable<CogenerationTariff> previousCogenerations)
    {
        var activeAeepp = GetActiveAverageElectricEnergyProductionPrice();

        GetActiveCogenerationTariffs().ForEach(ctf =>
        {
            var previousCogeneration =
                CogenerationTariffByProjectTypeAndNgsp(ctf.ProjectTypeId) ??
                CogenerationTariffByProjectTypeAndAeepp(ctf.ProjectTypeId);

            ctf.NaturalGasSellingPriceCorrection(
                _cogenerationParameterService, activeAeepp, correctedNgsp, previousCogeneration);

            _unitOfWork.Update(ctf);
            _unitOfWork.Update(previousCogeneration);

            LogNewCogenerationTariffCorrection(ctf);
        });

        CogenerationTariff CogenerationTariffByProjectTypeAndNgsp(Guid projectTypeId) =>
            previousCogenerations.SingleOrDefault(ctf => ctf.ProjectTypeId.Equals(projectTypeId) &&
                ctf.NaturalGasSellingPrice.Id.Equals(correctedNgsp.Id));

        CogenerationTariff CogenerationTariffByProjectTypeAndAeepp(Guid projectTypeId) =>
            previousCogenerations.Single(ctf => ctf.ProjectTypeId.Equals(projectTypeId) &&
                ctf.AverageElectricEnergyProductionPrice.Id.Equals(activeAeepp.Id));
    }

    private List<CogenerationTariff> GetActiveCogenerationTariffs() =>
        _repository.GetAll(new ActiveSpecification<CogenerationTariff>()).ToList();

    private AverageElectricEnergyProductionPrice GetActiveAverageElectricEnergyProductionPrice() =>
        _repository.GetSingle(new ActiveSpecification<AverageElectricEnergyProductionPrice>());

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

    private void LogNaturalGasSellingPriceCorrection(NaturalGasSellingPrice naturalGasSellingPrice) =>
        Log(new EntityExecutionLoggingEventArgs
        (
            SepsMessage.ParameterCorrection(
                nameof(NaturalGasSellingPrice),
                naturalGasSellingPrice.Active.Since.Date,
                naturalGasSellingPrice.Active.Until,
                naturalGasSellingPrice.Amount)
        ));
}

public sealed class CorrectActiveNaturalGasSellingPriceCommand : ISepsCommand
{
    public decimal Amount { get; set; }
    public string Remark { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}