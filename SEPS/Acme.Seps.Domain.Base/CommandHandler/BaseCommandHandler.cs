using System;
using Acme.Domain.Base.Entity;

namespace Acme.Seps.Domain.Base.CommandHandler
{
    public abstract class BaseCommandHandler
    {
        public event EventHandler<EntityExecutionLoggingEventArgs> UseCaseExecutionProcessing = delegate { };

        protected void Log(EntityExecutionLoggingEventArgs useCaseExecutionProcessingLog) =>
            UseCaseExecutionProcessing(this, useCaseExecutionProcessingLog);

        protected void LogSuccessfulCommit() =>
            Log(new EntityExecutionLoggingEventArgs { Message = Infrastructure.Base.SuccessfulSave });
    }
}