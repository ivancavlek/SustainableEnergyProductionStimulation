using System;
using Acme.Domain.Base.Command;
using Acme.Domain.Base.CommandHandler;
using Acme.Domain.Base.Entity;

namespace Acme.Seps.Domain.Base.CommandHandler
{
    public interface ISepsCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : BaseCommand
    {
        event EventHandler<EntityExecutionLoggingEventArgs> UseCaseExecutionProcessing;
    }
}