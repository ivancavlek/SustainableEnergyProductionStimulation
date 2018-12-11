using System;

namespace Acme.Seps.Domain.Base.Entity
{
    public class EntityExecutionLoggingEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}