using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Text;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Base.Test.Unit.CommandHandler
{
    public class BaseCommandHandlerTests
    {
        private readonly DummyCommandHandler _dummyCommandHandler;
        private readonly ICollection<string> _receivedMessages;

        public BaseCommandHandlerTests()
        {
            _dummyCommandHandler = new DummyCommandHandler();
            _receivedMessages = new List<string>();

            _dummyCommandHandler.UseCaseExecutionProcessing += (object sender, EntityExecutionLoggingEventArgs e) =>
                _receivedMessages.Add(e.Message);
        }

        public void LogIsWritten()
        {
            const string message = "Test message";

            _dummyCommandHandler.TestLog(new EntityExecutionLoggingEventArgs { Message = message });

            _receivedMessages.Count.Should().Be(1);
            _receivedMessages.ElementAt(0).Should().Be(message);
        }

        public void LogSuccessfulCommitIsWritten()
        {
            _dummyCommandHandler.TestSaveLog();

            _receivedMessages.Count.Should().Be(1);
            _receivedMessages.ElementAt(0).Should().Be(SepsBaseMessage.SuccessfulSave);
        }

        private class DummyCommandHandler : BaseCommandHandler
        {
            public void TestLog(EntityExecutionLoggingEventArgs test) =>
                base.Log(test);

            public void TestSaveLog() =>
                base.LogSuccessfulCommit();
        }
    }
}