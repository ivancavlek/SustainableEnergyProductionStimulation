using Acme.Domain.Base.Entity;
using System;

namespace Acme.Seps.Domain.Base.ApplicationService
{
    public abstract class SepsBaseService
    {
        public event EventHandler<EntityExecutionLoggingEventArgs> UseCaseExecutionProcessing = delegate { };

        protected void Log(EntityExecutionLoggingEventArgs useCaseExecutionProcessingLog) =>
            UseCaseExecutionProcessing(this, useCaseExecutionProcessingLog);
    }
}