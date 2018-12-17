using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Subsidy.Command;
using Acme.Seps.Domain.Subsidy.Command.Entity;
using Acme.Seps.Domain.Subsidy.Command.Repository;
using Acme.Seps.Domain.Subsidy.Test.Unit.Factory;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.CommandHandler
{
    public class CorrectActiveCpiCommandHandlerTests
    {
        private readonly ISepsCommandHandler<CorrectActiveConsumerPriceIndexCommand> _correctActiveCpi;
        private readonly IUnitOfWork _unitOfWork;

        public CorrectActiveCpiCommandHandlerTests()
        {
            IEconometricIndexFactory<ConsumerPriceIndex> cpiFactory =
                new EconometricIndexFactory<ConsumerPriceIndex>(DateTime.Now.AddYears(-4));
            var activeCpi = cpiFactory.Create();

            ITariffFactory<RenewableEnergySourceTariff> resFactory =
                new TariffFactory<RenewableEnergySourceTariff>(activeCpi);
            var activeResTariffs = new List<RenewableEnergySourceTariff> { resFactory.Create() };

            cpiFactory = new EconometricIndexFactory<ConsumerPriceIndex>(activeCpi.Active.Since.AddYears(-1));
            var previousActiveCpi = cpiFactory.Create();

            resFactory = new TariffFactory<RenewableEnergySourceTariff>(previousActiveCpi);
            var previousActiveResTariffs = new List<RenewableEnergySourceTariff> { resFactory.Create() };

            var dummyGuid = Guid.NewGuid();
            const string projectTypeIdProperty = "ProjectTypeId";
            typeof(RenewableEnergySourceTariff).BaseType
                .GetProperty(projectTypeIdProperty).SetValue(activeResTariffs[0], dummyGuid);
            typeof(RenewableEnergySourceTariff).BaseType
                .GetProperty(projectTypeIdProperty).SetValue(previousActiveResTariffs[0], dummyGuid);

            var repository = Substitute.For<IRepository>();
            repository
                .GetSingle(Arg.Any<ActiveSpecification<ConsumerPriceIndex>>())
                .Returns(activeCpi);
            repository
                .GetAll(Arg.Any<PreviousActiveSpecification<RenewableEnergySourceTariff>>())
                .Returns(previousActiveResTariffs);
            repository
                .GetAll(Arg.Any<ActiveSpecification<RenewableEnergySourceTariff>>())
                .Returns(activeResTariffs);

            _unitOfWork = Substitute.For<IUnitOfWork>();

            _correctActiveCpi = new CorrectActiveCpiCommandHandler(
                repository, _unitOfWork, Substitute.For<IIdentityFactory<Guid>>());
        }

        public void ExecutesProperly()
        {
            var correctActiveCpi = new CorrectActiveConsumerPriceIndexCommand
            {
                Amount = 100M,
                Remark = nameof(CalculateConsumerPriceIndexCommand)
            };

            using (var monitoredEvent = _correctActiveCpi.Monitor())
            {
                _correctActiveCpi.Handle(correctActiveCpi);

                _unitOfWork.Received().Update(Arg.Any<RenewableEnergySourceTariff>());
                _unitOfWork.Received().Commit();
                monitoredEvent.Should().Raise("UseCaseExecutionProcessing");
            }
        }
    }
}