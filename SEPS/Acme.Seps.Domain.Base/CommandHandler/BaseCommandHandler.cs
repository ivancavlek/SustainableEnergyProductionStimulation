using Acme.Domain.Base.Entity;
using Acme.Seps.Domain.Base.ApplicationService;
using System;

namespace Acme.Seps.Domain.Base.CommandHandler
{
    public abstract class BaseCommandHandler
    {
        protected readonly ISepsLogService SepsLogService;

        protected BaseCommandHandler(ISepsLogService sepsLogService)
        {
            if (sepsLogService == null)
                throw new ArgumentNullException(nameof(sepsLogService));

            SepsLogService = sepsLogService;
        }

        protected void LogSuccessfulCommit() =>
            SepsLogService.Log(new EntityExecutionLoggingEventArgs { Message = Infrastructure.Base.SuccessfulSave });
    }
}