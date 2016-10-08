using System;

namespace Acme.Seps.Domain.Base.ApplicationService
{
    public abstract class SepsBaseService
    {
        public event EventHandler<string> UseCaseExecutionProcessing = delegate { };

        protected void Log(string useCaseExecutionProcessingLog) =>
            UseCaseExecutionProcessing(this, useCaseExecutionProcessingLog);
    }
}