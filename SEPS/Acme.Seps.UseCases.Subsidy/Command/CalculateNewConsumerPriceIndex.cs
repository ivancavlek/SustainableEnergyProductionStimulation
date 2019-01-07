using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.UseCases.Subsidy.Command
{
    public sealed class CalculateCpiCommandHandler
        : BaseCommandHandler, ISepsCommandHandler<CalculateConsumerPriceIndexCommand>
    {
        public CalculateCpiCommandHandler(
            IRepository repository, IUnitOfWork unitOfWork, IIdentityFactory<Guid> identityFactory)
            : base(repository, unitOfWork, identityFactory) { }

        void ICommandHandler<CalculateConsumerPriceIndexCommand>.Handle(CalculateConsumerPriceIndexCommand command)
        {
            var activeCpi = GetActiveConsumerPriceIndex();
            var newCpi = CreateNewConsumerPriceIndex(activeCpi, command);

            CreateNewRenewableEnergySourceTariffs(newCpi);

            Commit();

            LogNewConsumerPriceIndex(newCpi);
            LogSuccessfulCommit();
        }

        private ConsumerPriceIndex GetActiveConsumerPriceIndex() =>
            _repository.GetSingle(new ActiveSpecification<ConsumerPriceIndex>());

        private ConsumerPriceIndex CreateNewConsumerPriceIndex(
            ConsumerPriceIndex activeCpi, CalculateConsumerPriceIndexCommand command)
        {
            var newCpi = activeCpi.CreateNew(command.Amount, command.Remark, _identityFactory);

            _unitOfWork.Update(activeCpi);
            _unitOfWork.Insert(newCpi);

            return newCpi;
        }

        private void CreateNewRenewableEnergySourceTariffs(ConsumerPriceIndex newCpi)
        {
            GetActiveRenewableEnergySourceTariffs().ForEach(res =>
            {
                var newRenewableEnergyTariff = CreateNewRenewableEnergySourceTariff(res);

                _unitOfWork.Update(res);
                _unitOfWork.Insert(newRenewableEnergyTariff);

                LogNewRenewableEnergySourceTariff(newRenewableEnergyTariff);
            });

            RenewableEnergySourceTariff CreateNewRenewableEnergySourceTariff(RenewableEnergySourceTariff res) =>
                res.CreateNewWith(newCpi, _identityFactory);
        }

        private List<RenewableEnergySourceTariff> GetActiveRenewableEnergySourceTariffs() =>
            _repository.GetAll(new ActiveSpecification<RenewableEnergySourceTariff>()).ToList();

        private void LogNewRenewableEnergySourceTariff(RenewableEnergySourceTariff renewableEnergySourceTariff) =>
            Log(new EntityExecutionLoggingEventArgs
            (
                SepsMessage.InsertTariff(
                    nameof(RenewableEnergySourceTariff),
                    renewableEnergySourceTariff.Active.Since.Date,
                    renewableEnergySourceTariff.Active.Until,
                    renewableEnergySourceTariff.LowerRate,
                    renewableEnergySourceTariff.HigherRate)
            ));

        private void LogNewConsumerPriceIndex(ConsumerPriceIndex consumerPriceIndex) =>
            Log(new EntityExecutionLoggingEventArgs
            (
                SepsMessage.InsertParameter(
                    nameof(ConsumerPriceIndex),
                    consumerPriceIndex.Active.Since.Date,
                    consumerPriceIndex.Active.Until,
                    consumerPriceIndex.Amount)
            ));
    }

    public sealed class CalculateConsumerPriceIndexCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}