﻿using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Base.Test.Unit.CommandHandler;

public class BaseCommandHandlerTests
{
    private readonly DummyCommandHandler _dummyCommandHandler;
    private readonly ICollection<string> _receivedMessages;

    public BaseCommandHandlerTests()
    {
        _dummyCommandHandler = new DummyCommandHandler(
            Substitute.For<IRepository>(), Substitute.For<IUnitOfWork>(), Substitute.For<IIdentityFactory<Guid>>());
        _receivedMessages = new List<string>();

        _dummyCommandHandler.UseCaseExecutionProcessing += (object sender, EntityExecutionLoggingEventArgs e) =>
            _receivedMessages.Add(e.Message);
    }

    public void RepositoryIsCorrectlyInitialized()
    {
        Action action = () => new DummyCommandHandler(
            null, Substitute.For<IUnitOfWork>(), Substitute.For<IIdentityFactory<Guid>>());

        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    public void UnitOfWorkIsCorrectlyInitialized()
    {
        Action action = () => new DummyCommandHandler(
            Substitute.For<IRepository>(), null, Substitute.For<IIdentityFactory<Guid>>());

        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    public void IdentityFactoryIsCorrectlyInitialized()
    {
        Action action = () => new DummyCommandHandler(
            Substitute.For<IRepository>(), Substitute.For<IUnitOfWork>(), null);

        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    public void LogIsWritten()
    {
        const string message = "Test message";

        _dummyCommandHandler.TestLog(new EntityExecutionLoggingEventArgs(message));

        _receivedMessages.Count.Should().Be(1);
        _receivedMessages.ElementAt(0).Should().Be(message);
    }

    public void LogSuccessfulCommitIsWritten()
    {
        _dummyCommandHandler.TestSaveLog();

        _receivedMessages.Count.Should().Be(1);
        _receivedMessages.ElementAt(0).Should().Be(SepsMessage.SuccessfulSave());
    }

    private class DummyCommandHandler : BaseCommandHandler
    {
        public DummyCommandHandler(
            IRepository repository, IUnitOfWork unitOfWork, IIdentityFactory<Guid> identityFactory)
            : base(repository, unitOfWork, identityFactory) { }

        public void TestLog(EntityExecutionLoggingEventArgs test) =>
            Log(test);

        public void TestSaveLog() =>
            LogSuccessfulCommit();
    }
}