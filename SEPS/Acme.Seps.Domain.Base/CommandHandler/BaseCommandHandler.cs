using Acme.Domain.Base.Entity;
using System;

namespace Acme.Seps.Domain.Base.CommandHandler
{
    public abstract class BaseCommandHandler : ISepsLogService
    {
        public event EventHandler<EntityExecutionLoggingEventArgs> UseCaseExecutionProcessing = delegate { };

        protected void Log(EntityExecutionLoggingEventArgs useCaseExecutionProcessingLog) =>
            UseCaseExecutionProcessing(this, useCaseExecutionProcessingLog);

        protected void LogSuccessfulCommit() =>
            Log(new EntityExecutionLoggingEventArgs { Message = Infrastructure.Base.SuccessfulSave });
    }
}