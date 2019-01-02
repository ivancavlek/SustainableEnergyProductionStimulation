using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Test.Unit.Utility.Factory;
using Acme.Seps.UseCases.Subsidy.Command;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace Acme.Seps.UseCases.Subsidy.Test.Unit.CommandHandler
{
    public class CalculateCpiCommandHandlerTests
    {
        private readonly ISepsCommandHandler<CalculateConsumerPriceIndexCommand> _calculateCpi;
        private readonly IUnitOfWork _unitOfWork;

        public CalculateCpiCommandHandlerTests()
        {
            IEconometricIndexFactory<ConsumerPriceIndex> cpiFactory =
                new EconometricIndexFactory<ConsumerPriceIndex>(DateTime.Now.AddYears(-4));
            var activeCpi = cpiFactory.Create();

            ITariffFactory<RenewableEnergySourceTariff> resFactory = new ResTariffFactory(activeCpi);
            var activeRenewableEnergySourceTariff = resFactory.Create();

            var repository = Substitute.For<IRepository>();
            repository
                .GetSingle(Arg.Any<ActiveSpecification<ConsumerPriceIndex>>())
                .Returns(activeCpi);
            repository
                .GetAll(Arg.Any<ActiveSpecification<RenewableEnergySourceTariff>>())
                .Returns(new List<RenewableEnergySourceTariff> { activeRenewableEnergySourceTariff });

            _unitOfWork = Substitute.For<IUnitOfWork>();

            _calculateCpi = new CalculateCpiCommandHandler(
                repository, _unitOfWork, Substitute.For<IIdentityFactory<Guid>>());
        }

        public void ExecutesProperly()
        {
            var calculateCommand = new CalculateConsumerPriceIndexCommand
            {
                Amount = 100M,
                Remark = nameof(CalculateConsumerPriceIndexCommand)
            };

            _calculateCpi.Handle(calculateCommand);

            _unitOfWork.Received().Update(Arg.Any<ConsumerPriceIndex>());
            _unitOfWork.Received().Insert(Arg.Any<ConsumerPriceIndex>());
            _unitOfWork.Received().Update(Arg.Any<RenewableEnergySourceTariff>());
            _unitOfWork.Received().Insert(Arg.Any<RenewableEnergySourceTariff>());
            _unitOfWork.Received().Commit();
        }
    }
}