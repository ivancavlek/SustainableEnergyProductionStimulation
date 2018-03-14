using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.ApplicationService;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Domain.Parameter.CommandHandler;
using Acme.Seps.Domain.Parameter.DomainService;
using Acme.Seps.Domain.Parameter.Entity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.CommandHandler
{
    public class CorrectActiveNaturalGasCommandHandlerTests
    {
        private readonly ICommandHandler<CorrectActiveNaturalGasCommand> _calculateNaturalGas;
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityFactory<Guid> _identityFactory;
        private readonly ISepsLogService _sepsLogService;

        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly IEnumerable<CogenerationTariff> _cogenerationTariff;
        private readonly DateTime _lastPeriod;

        public CorrectActiveNaturalGasCommandHandlerTests()
        {
            _sepsLogService = Substitute.For<ISepsLogService>();
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _identityFactory.CreateIdentity().Returns(Guid.NewGuid());

            _lastPeriod = DateTime.Now.AddMonths(-3);

            _cogenerationParameterService = Substitute.For<ICogenerationParameterService>();
            _cogenerationParameterService
                .GetFrom(Arg.Any<IEnumerable<NaturalGasSellingPrice>>(), Arg.Any<NaturalGasSellingPrice>())
                .Returns(1M);

            _naturalGasSellingPrice = Activator.CreateInstance(
                typeof(NaturalGasSellingPrice),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    10M,
                    nameof(NaturalGasSellingPrice),
                    new MonthlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
                    _identityFactory },
                null) as NaturalGasSellingPrice;

            _cogenerationTariff = new List<CogenerationTariff> { Activator.CreateInstance(
                    typeof(CogenerationTariff),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    _naturalGasSellingPrice,
                    10M,
                    10M,
                    new MonthlyPeriod(DateTime.Now.AddMonths(-4), _lastPeriod),
                    _identityFactory },
                null) as CogenerationTariff };

            _unitOfWork = Substitute.For<IUnitOfWork>();

            _calculateNaturalGas = new CorrectActiveNaturalGasCommandHandler(
                _cogenerationParameterService, _unitOfWork, _identityFactory, _sepsLogService);
        }

        public void ExecutesProperly()
        {
            var correctActiveNaturalGasCommand = new CorrectActiveNaturalGasCommand
            {
                ActiveNaturalGasSellingPrice = _naturalGasSellingPrice,
                ActiveCogenerationTariffs = _cogenerationTariff,
                Amount = 100M,
                Month = _lastPeriod.Month,
                Remark = nameof(CalculateNaturalGasCommand),
                Year = _lastPeriod.Year,
                YearsNaturalGasSellingPrices = new List<NaturalGasSellingPrice> { _naturalGasSellingPrice }
            };

            _calculateNaturalGas.Handle(correctActiveNaturalGasCommand);

            _unitOfWork.Received().Insert(Arg.Any<NaturalGasSellingPrice>());
            _unitOfWork.Received().Delete(Arg.Any<NaturalGasSellingPrice>());
            _unitOfWork.Received().Insert(Arg.Any<CogenerationTariff>());
            _unitOfWork.Received().Delete(Arg.Any<CogenerationTariff>());
            _unitOfWork.Received().Commit();
            _sepsLogService.Received(3).Log(Arg.Any<EntityExecutionLoggingEventArgs>());
        }
    }
}