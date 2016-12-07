using Acme.Domain.Base.Entity;
using System;

namespace Acme.Seps.Domain.Base.ApplicationService
{
    public interface ISepsLogService
    {
        event EventHandler<EntityExecutionLoggingEventArgs> UseCaseExecutionProcessing;

        void Log(EntityExecutionLoggingEventArgs useCaseExecutionProcessingLog);
    }
}