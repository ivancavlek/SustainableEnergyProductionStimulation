using System;

namespace Acme.Seps.Domain.Base.CommandHandler
{
    public class EntityExecutionLoggingEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}