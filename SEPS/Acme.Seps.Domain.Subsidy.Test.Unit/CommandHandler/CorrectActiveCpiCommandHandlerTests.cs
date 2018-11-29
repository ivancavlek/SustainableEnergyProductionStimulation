﻿//using Acme.Domain.Base.Factory;
//using Acme.Domain.Base.Repository;
//using Acme.Seps.Domain.Base.CommandHandler;
//using Acme.Seps.Domain.Base.Factory;
//using Acme.Seps.Domain.Base.ValueType;
//using Acme.Seps.Domain.Parameter.Command;
//using Acme.Seps.Domain.Parameter.CommandHandler;
//using Acme.Seps.Domain.Parameter.Entity;
//using FluentAssertions;
//using NSubstitute;
//using System;
//using System.Collections.Generic;
//using System.Reflection;

//namespace Acme.Seps.Domain.Subsidy.Test.Unit.CommandHandler
//{
//    public class CorrectActiveCpiCommandHandlerTests
//    {
//        private readonly ISepsCommandHandler<CorrectActiveCpiCommand> _correctActiveCpi;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IIdentityFactory<Guid> _identityFactory;

//        private readonly ConsumerPriceIndex _cpi;
//        private readonly IEnumerable<RenewableEnergySourceTariff> _resTariff;

//        public CorrectActiveCpiCommandHandlerTests()
//        {
//            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
//            _identityFactory.CreateIdentity().Returns(Guid.NewGuid());

//            _cpi = Activator.CreateInstance(
//                typeof(ConsumerPriceIndex),
//                BindingFlags.Instance | BindingFlags.NonPublic,
//                null,
//                new object[]
//                {
//                    10M,
//                    nameof(ConsumerPriceIndex),
//                    new Period(new YearlyPeriodFactory(DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-3))),
//                    _identityFactory
//                },
//                null) as ConsumerPriceIndex;

//            _resTariff = new List<RenewableEnergySourceTariff> { Activator.CreateInstance(
//                typeof(RenewableEnergySourceTariff),
//                BindingFlags.Instance | BindingFlags.NonPublic,
//                null,
//                new object[]
//                {
//                    _cpi,
//                    100,
//                    500,
//                    5M,
//                    10M,
//                    new YearlyPeriodFactory(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
//                    _identityFactory
//                },
//                null) as RenewableEnergySourceTariff };

//            _unitOfWork = Substitute.For<IUnitOfWork>();

//            _correctActiveCpi = new CorrectActiveCpiCommandHandler(_unitOfWork, _identityFactory);
//        }

//        public void ExecutesProperly()
//        {
//            var correctActiveCpi = new CorrectActiveCpiCommand
//            {
//                Amount = 100M,
//                Remark = nameof(CalculateCpiCommand)
//            };

//            using (var monitoredEvent = _correctActiveCpi.Monitor())
//            {
//                _correctActiveCpi.Handle(correctActiveCpi);

//                _unitOfWork.Received().Delete(Arg.Any<ConsumerPriceIndex>());
//                _unitOfWork.Received().Insert(Arg.Any<ConsumerPriceIndex>());
//                _unitOfWork.Received().Delete(Arg.Any<RenewableEnergySourceTariff>());
//                _unitOfWork.Received().Insert(Arg.Any<RenewableEnergySourceTariff>());
//                _unitOfWork.Received().Commit();
//                monitoredEvent.Should().Raise("UseCaseExecutionProcessing");
//            }
//        }
//    }
//}