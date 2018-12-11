using Acme.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Entity;
using System;

namespace Acme.Seps.Domain.Base.CommandHandler
{
    public interface ISepsCommandHandler<TCommand> : ICommandHandler<TCommand>
    {
        event EventHandler<EntityExecutionLoggingEventArgs> UseCaseExecutionProcessing;
    }
}