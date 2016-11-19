using System;

namespace Acme.Domain.Base.Entity
{
    public class EntityExecutionLoggingEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}