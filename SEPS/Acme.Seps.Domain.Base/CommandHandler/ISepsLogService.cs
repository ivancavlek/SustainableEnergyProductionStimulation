using Acme.Domain.Base.Entity;
using System;

namespace Acme.Seps.Domain.Base.CommandHandler
{
    public interface ISepsLogService
    {
        event EventHandler<EntityExecutionLoggingEventArgs> UseCaseExecutionProcessing;
    }
}