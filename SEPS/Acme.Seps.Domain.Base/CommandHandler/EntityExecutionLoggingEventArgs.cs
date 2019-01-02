using System;

namespace Acme.Seps.Domain.Base.CommandHandler
{
    public sealed class EntityExecutionLoggingEventArgs : EventArgs
    {
        public string Message { get; }

        public EntityExecutionLoggingEventArgs(string message) =>
            Message = message;
    }
}