using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.UseCases.Subsidy.Command;

public sealed class CorrectActiveCpiCommandHandler
    : BaseCommandHandler, ISepsCommandHandler<CorrectActiveConsumerPriceIndexCommand>
{
    public CorrectActiveCpiCommandHandler(
        IRepository repository, IUnitOfWork unitOfWork, IIdentityFactory<Guid> identityFactory)
        : base(repository, unitOfWork, identityFactory) { }

    void ICommandHandler<CorrectActiveConsumerPriceIndexCommand>.Handle(
        CorrectActiveConsumerPriceIndexCommand command)
    {
        var activeCpi = GetActiveConsumerPriceIndex();
        var previousRes = GetPreviousActiveRenewableEnergySourceTariffs(activeCpi);

        activeCpi.Correct(command.Amount, command.Remark);
        CorrectRenewableEnergySourceTariffs(activeCpi, previousRes);

        Commit();

        LogConsumerPriceIndexCorrection(activeCpi);
        LogSuccessfulCommit();
    }

    private ConsumerPriceIndex GetActiveConsumerPriceIndex() =>
        _repository.GetSingle(new ActiveSpecification<ConsumerPriceIndex>());

    private IReadOnlyList<RenewableEnergySourceTariff> GetPreviousActiveRenewableEnergySourceTariffs(
        ConsumerPriceIndex cpi) =>
        _repository.GetAll(new PreviousActiveSpecification<RenewableEnergySourceTariff>(cpi));

    private void CorrectRenewableEnergySourceTariffs(
        ConsumerPriceIndex correctedCpi, IEnumerable<RenewableEnergySourceTariff> previousRes)
    {
        GetActiveRenewableEnergySourceTariffs().ForEach(res =>
        {
            res.CpiCorrection(
                correctedCpi, PreviousRenewableEnergySourceBy(res.ProjectTypeId, res.LowerProductionLimit));

            _unitOfWork.Update(res);

            LogRenewableEnergySourceTariffCorrection(res);
        });

        RenewableEnergySourceTariff PreviousRenewableEnergySourceBy(
            Guid projectTypeId, decimal? lowerProductionLimit) =>
            previousRes.Single(res =>
                res.ProjectTypeId.Equals(projectTypeId) && res.LowerProductionLimit.Equals(lowerProductionLimit));
    }

    private List<RenewableEnergySourceTariff> GetActiveRenewableEnergySourceTariffs() =>
        _repository.GetAll(new ActiveSpecification<RenewableEnergySourceTariff>()).ToList();

    private void LogRenewableEnergySourceTariffCorrection(
        RenewableEnergySourceTariff renewableEnergySourceTariff) =>
        Log(new EntityExecutionLoggingEventArgs
        (
            SepsMessage.TariffCorrection(
                nameof(RenewableEnergySourceTariff),
                renewableEnergySourceTariff.Active.Since.Date,
                renewableEnergySourceTariff.Active.Until,
                renewableEnergySourceTariff.LowerRate,
                renewableEnergySourceTariff.HigherRate)
        ));

    private void LogConsumerPriceIndexCorrection(ConsumerPriceIndex consumerPriceIndex) =>
        Log(new EntityExecutionLoggingEventArgs
        (
            SepsMessage.ParameterCorrection(
                nameof(ConsumerPriceIndex),
                consumerPriceIndex.Active.Since.Date,
                consumerPriceIndex.Active.Until,
                consumerPriceIndex.Amount)
        ));
}

public sealed class CorrectActiveConsumerPriceIndexCommand : ISepsCommand
{
    public decimal Amount { get; set; }
    public string Remark { get; set; }
}